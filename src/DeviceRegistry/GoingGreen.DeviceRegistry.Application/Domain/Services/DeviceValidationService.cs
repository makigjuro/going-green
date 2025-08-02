using GoingGreen.DeviceRegistry.Application.Domain.ValueObjects;

namespace GoingGreen.DeviceRegistry.Application.Domain.Services;

public class DeviceValidationService
{
    private static readonly HashSet<string> AcceptedDeviceTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Smartphone",
        "Tablet", 
        "Laptop",
        "Desktop",
        "Monitor",
        "Gaming Console",
        "Smart Watch",
        "Headphones",
        "Camera"
    };

    private static readonly HashSet<string> AcceptedConditions = new(StringComparer.OrdinalIgnoreCase)
    {
        "Excellent",
        "Good", 
        "Fair",
        "Poor"
    };

    private static readonly Dictionary<string, decimal> BaseValues = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Smartphone", 200m },
        { "Tablet", 150m },
        { "Laptop", 400m },
        { "Desktop", 300m },
        { "Monitor", 100m },
        { "Gaming Console", 250m },
        { "Smart Watch", 100m },
        { "Headphones", 50m },
        { "Camera", 300m }
    };

    public (bool IsValid, string ValidationMessage, decimal BaseValue) ValidateDevice(DeviceInfo deviceInfo)
    {
        if (!AcceptedDeviceTypes.Contains(deviceInfo.Type))
        {
            return (false, $"Device type '{deviceInfo.Type}' is not accepted for quotes.", 0m);
        }

        if (!AcceptedConditions.Contains(deviceInfo.Condition))
        {
            return (false, $"Device condition '{deviceInfo.Condition}' is not valid. Accepted conditions: {string.Join(", ", AcceptedConditions)}", 0m);
        }

        if (deviceInfo.Age > 10)
        {
            return (false, "Devices older than 10 years are not accepted for quotes.", 0m);
        }

        if (string.IsNullOrWhiteSpace(deviceInfo.Brand) || string.IsNullOrWhiteSpace(deviceInfo.Model))
        {
            return (false, "Device brand and model are required.", 0m);
        }

        var baseValue = BaseValues.GetValueOrDefault(deviceInfo.Type, 0m);
        return (true, "Device is valid for quote calculation.", baseValue);
    }
}