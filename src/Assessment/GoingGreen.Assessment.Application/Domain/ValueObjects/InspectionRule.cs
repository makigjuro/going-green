namespace GoingGreen.Assessment.Application.Domain.ValueObjects;

public record InspectionRule(
    string Name,
    string DeviceType,
    string Criterion,
    InspectionRuleType Type,
    object ExpectedValue,
    decimal ImpactMultiplier,
    bool IsCritical)
{
    public static InspectionRule Create(
        string name,
        string deviceType,
        string criterion,
        InspectionRuleType type,
        object expectedValue,
        decimal impactMultiplier,
        bool isCritical = false)
    {
        return new InspectionRule(name, deviceType, criterion, type, expectedValue, impactMultiplier, isCritical);
    }
}

public enum InspectionRuleType
{
    Boolean,        // true/false (e.g., "Powers On")
    Numeric,        // numeric value (e.g., "Battery Health %")
    Text,           // text match (e.g., "Screen Condition")
    Range,          // numeric range (e.g., "Age in months")
    Enumeration     // specific values (e.g., "Excellent", "Good", "Fair", "Poor")
}

public record InspectionCriterion(
    string Name,
    InspectionRuleType Type,
    string Description,
    List<string>? AllowedValues = null)
{
    public static InspectionCriterion Boolean(string name, string description)
        => new(name, InspectionRuleType.Boolean, description);

    public static InspectionCriterion Numeric(string name, string description)
        => new(name, InspectionRuleType.Numeric, description);

    public static InspectionCriterion Text(string name, string description)
        => new(name, InspectionRuleType.Text, description);

    public static InspectionCriterion Enumeration(string name, string description, List<string> allowedValues)
        => new(name, InspectionRuleType.Enumeration, description, allowedValues);
}