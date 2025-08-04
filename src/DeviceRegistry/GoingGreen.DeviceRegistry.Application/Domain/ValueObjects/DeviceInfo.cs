namespace GoingGreen.DeviceRegistry.Application.Domain.ValueObjects;

public record DeviceInfo(
    string Type,
    string Condition,
    string Brand,
    string Model,
    int Age)
{
    public static DeviceInfo Create(string type, string condition, string brand, string model, int age)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Device type cannot be empty", nameof(type));
        
        if (string.IsNullOrWhiteSpace(condition))
            throw new ArgumentException("Device condition cannot be empty", nameof(condition));
        
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Device brand cannot be empty", nameof(brand));
        
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Device model cannot be empty", nameof(model));
        
        if (age < 0)
            throw new ArgumentException("Device age cannot be negative", nameof(age));

        return new DeviceInfo(type, condition, brand, model, age);
    }
}