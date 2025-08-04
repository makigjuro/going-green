using GoingGreen.Assessment.Application.Domain.ValueObjects;

namespace GoingGreen.Assessment.Application.Domain.Services;

public interface IInspectionRulesEngine
{
    List<InspectionRule> GetRulesForDevice(string deviceType);
    List<InspectionCriterion> GetInspectionCriteria(string deviceType);
    ClassificationResult ClassifyDevice(string deviceType, Dictionary<string, object> inspectionResults, decimal originalValue);
    decimal CalculateValueAdjustment(string deviceType, Dictionary<string, object> inspectionResults, decimal originalValue);
}

public class InspectionRulesEngine : IInspectionRulesEngine
{
    private readonly Dictionary<string, List<InspectionRule>> _deviceRules;
    private readonly Dictionary<string, List<InspectionCriterion>> _deviceCriteria;

    public InspectionRulesEngine()
    {
        _deviceRules = InitializeDeviceRules();
        _deviceCriteria = InitializeDeviceCriteria();
    }

    public List<InspectionRule> GetRulesForDevice(string deviceType)
    {
        return _deviceRules.GetValueOrDefault(deviceType.ToLowerInvariant(), new List<InspectionRule>());
    }

    public List<InspectionCriterion> GetInspectionCriteria(string deviceType)
    {
        return _deviceCriteria.GetValueOrDefault(deviceType.ToLowerInvariant(), new List<InspectionCriterion>());
    }

    public ClassificationResult ClassifyDevice(string deviceType, Dictionary<string, object> inspectionResults, decimal originalValue)
    {
        var rules = GetRulesForDevice(deviceType);
        var failedCriticalRules = new List<string>();
        var adjustmentFactor = 1.0m;

        foreach (var rule in rules)
        {
            if (!inspectionResults.TryGetValue(rule.Criterion, out var actualValue))
                continue;

            var ruleResult = EvaluateRule(rule, actualValue);
            
            if (!ruleResult.Passed)
            {
                if (rule.IsCritical)
                {
                    failedCriticalRules.Add(rule.Name);
                }
                adjustmentFactor *= rule.ImpactMultiplier;
            }
        }

        // If critical rules failed, device goes to recycling
        if (failedCriticalRules.Any())
        {
            var recyclingValue = originalValue * 0.1m; // 10% for recycling
            return ClassificationResult.Recycle(recyclingValue, $"Critical issues: {string.Join(", ", failedCriticalRules)}");
        }

        var adjustedValue = originalValue * adjustmentFactor;

        // Determine resale grade based on adjustment factor
        var resaleGrade = adjustmentFactor switch
        {
            >= 0.9m => ResaleGrade.A,
            >= 0.7m => ResaleGrade.B,
            >= 0.5m => ResaleGrade.C,
            >= 0.3m => ResaleGrade.D,
            _ => ResaleGrade.D
        };

        // If value drops too low, recycle instead
        if (adjustedValue < originalValue * 0.3m)
        {
            var recyclingValue = originalValue * 0.15m;
            return ClassificationResult.Recycle(recyclingValue, "Value too low for resale market");
        }

        return ClassificationResult.Resellable(resaleGrade, adjustedValue, $"Resale grade {resaleGrade} with {adjustmentFactor:P0} value retention");
    }

    public decimal CalculateValueAdjustment(string deviceType, Dictionary<string, object> inspectionResults, decimal originalValue)
    {
        var result = ClassifyDevice(deviceType, inspectionResults, originalValue);
        return result.Value - originalValue;
    }

    private (bool Passed, string Reason) EvaluateRule(InspectionRule rule, object actualValue)
    {
        return rule.Type switch
        {
            InspectionRuleType.Boolean => EvaluateBooleanRule(rule, actualValue),
            InspectionRuleType.Numeric => EvaluateNumericRule(rule, actualValue),
            InspectionRuleType.Text => EvaluateTextRule(rule, actualValue),
            InspectionRuleType.Enumeration => EvaluateEnumerationRule(rule, actualValue),
            _ => (false, "Unknown rule type")
        };
    }

    private (bool Passed, string Reason) EvaluateBooleanRule(InspectionRule rule, object actualValue)
    {
        if (actualValue is bool boolValue && rule.ExpectedValue is bool expectedBool)
        {
            var passed = boolValue == expectedBool;
            return (passed, passed ? "Passed" : $"Expected {expectedBool}, got {boolValue}");
        }
        return (false, "Invalid boolean value");
    }

    private (bool Passed, string Reason) EvaluateNumericRule(InspectionRule rule, object actualValue)
    {
        if (actualValue is decimal numValue && rule.ExpectedValue is decimal expectedNum)
        {
            var passed = numValue >= expectedNum;
            return (passed, passed ? "Passed" : $"Expected >= {expectedNum}, got {numValue}");
        }
        return (false, "Invalid numeric value");
    }

    private (bool Passed, string Reason) EvaluateTextRule(InspectionRule rule, object actualValue)
    {
        if (actualValue is string textValue && rule.ExpectedValue is string expectedText)
        {
            var passed = string.Equals(textValue, expectedText, StringComparison.OrdinalIgnoreCase);
            return (passed, passed ? "Passed" : $"Expected '{expectedText}', got '{textValue}'");
        }
        return (false, "Invalid text value");
    }

    private (bool Passed, string Reason) EvaluateEnumerationRule(InspectionRule rule, object actualValue)
    {
        if (actualValue is string enumValue && rule.ExpectedValue is List<string> allowedValues)
        {
            var passed = allowedValues.Contains(enumValue, StringComparer.OrdinalIgnoreCase);
            return (passed, passed ? "Passed" : $"Expected one of [{string.Join(", ", allowedValues)}], got '{enumValue}'");
        }
        return (false, "Invalid enumeration value");
    }

    private Dictionary<string, List<InspectionRule>> InitializeDeviceRules()
    {
        return new Dictionary<string, List<InspectionRule>>(StringComparer.OrdinalIgnoreCase)
        {
            ["smartphone"] = new List<InspectionRule>
            {
                InspectionRule.Create("Powers On", "smartphone", "PowersOn", InspectionRuleType.Boolean, true, 0.0m, true),
                InspectionRule.Create("Screen Functional", "smartphone", "ScreenFunctional", InspectionRuleType.Boolean, true, 0.5m, true),
                InspectionRule.Create("Battery Health", "smartphone", "BatteryHealth", InspectionRuleType.Numeric, 70m, 0.8m, false),
                InspectionRule.Create("Physical Condition", "smartphone", "PhysicalCondition", InspectionRuleType.Enumeration, 
                    new List<string> { "Excellent", "Good", "Fair" }, 0.7m, false),
                InspectionRule.Create("Water Damage", "smartphone", "WaterDamage", InspectionRuleType.Boolean, false, 0.3m, true)
            },
            ["tablet"] = new List<InspectionRule>
            {
                InspectionRule.Create("Powers On", "tablet", "PowersOn", InspectionRuleType.Boolean, true, 0.0m, true),
                InspectionRule.Create("Screen Functional", "tablet", "ScreenFunctional", InspectionRuleType.Boolean, true, 0.4m, true),
                InspectionRule.Create("Battery Health", "tablet", "BatteryHealth", InspectionRuleType.Numeric, 60m, 0.8m, false),
                InspectionRule.Create("Physical Condition", "tablet", "PhysicalCondition", InspectionRuleType.Enumeration,
                    new List<string> { "Excellent", "Good", "Fair" }, 0.7m, false)
            },
            ["laptop"] = new List<InspectionRule>
            {
                InspectionRule.Create("Powers On", "laptop", "PowersOn", InspectionRuleType.Boolean, true, 0.0m, true),
                InspectionRule.Create("Display Functional", "laptop", "DisplayFunctional", InspectionRuleType.Boolean, true, 0.6m, true),
                InspectionRule.Create("Keyboard Functional", "laptop", "KeyboardFunctional", InspectionRuleType.Boolean, true, 0.8m, false),
                InspectionRule.Create("Battery Health", "laptop", "BatteryHealth", InspectionRuleType.Numeric, 50m, 0.9m, false),
                InspectionRule.Create("Physical Condition", "laptop", "PhysicalCondition", InspectionRuleType.Enumeration,
                    new List<string> { "Excellent", "Good", "Fair" }, 0.7m, false)
            }
        };
    }

    private Dictionary<string, List<InspectionCriterion>> InitializeDeviceCriteria()
    {
        return new Dictionary<string, List<InspectionCriterion>>(StringComparer.OrdinalIgnoreCase)
        {
            ["smartphone"] = new List<InspectionCriterion>
            {
                InspectionCriterion.Boolean("PowersOn", "Does the device power on?"),
                InspectionCriterion.Boolean("ScreenFunctional", "Is the screen fully functional with no dead pixels?"),
                InspectionCriterion.Numeric("BatteryHealth", "Battery health percentage (0-100)"),
                InspectionCriterion.Enumeration("PhysicalCondition", "Overall physical condition", 
                    new List<string> { "Excellent", "Good", "Fair", "Poor" }),
                InspectionCriterion.Boolean("WaterDamage", "Is there evidence of water damage?")
            },
            ["tablet"] = new List<InspectionCriterion>
            {
                InspectionCriterion.Boolean("PowersOn", "Does the device power on?"),
                InspectionCriterion.Boolean("ScreenFunctional", "Is the screen fully functional?"),
                InspectionCriterion.Numeric("BatteryHealth", "Battery health percentage (0-100)"),
                InspectionCriterion.Enumeration("PhysicalCondition", "Overall physical condition",
                    new List<string> { "Excellent", "Good", "Fair", "Poor" })
            },
            ["laptop"] = new List<InspectionCriterion>
            {
                InspectionCriterion.Boolean("PowersOn", "Does the device power on?"),
                InspectionCriterion.Boolean("DisplayFunctional", "Is the display fully functional?"),
                InspectionCriterion.Boolean("KeyboardFunctional", "Are all keyboard keys working?"),
                InspectionCriterion.Numeric("BatteryHealth", "Battery health percentage (0-100)"),
                InspectionCriterion.Enumeration("PhysicalCondition", "Overall physical condition",
                    new List<string> { "Excellent", "Good", "Fair", "Poor" })
            }
        };
    }
}