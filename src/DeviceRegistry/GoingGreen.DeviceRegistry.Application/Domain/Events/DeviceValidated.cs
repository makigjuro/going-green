namespace GoingGreen.DeviceRegistry.Application.Domain.Events;

public class DeviceValidated
{
    public Guid QuoteId { get; init; }
    public bool IsValid { get; init; }
    public string ValidationMessage { get; init; } = string.Empty;
    public string DeviceType { get; init; } = string.Empty;
    public string DeviceCondition { get; init; } = string.Empty;
    public string DeviceBrand { get; init; } = string.Empty;
    public string DeviceModel { get; init; } = string.Empty;
    public int DeviceAge { get; init; }
    public decimal BaseValue { get; init; }
    public DateTime ValidatedAt { get; init; }

    public DeviceValidated() { }

    public DeviceValidated(
        Guid quoteId,
        bool isValid,
        string validationMessage,
        string deviceType,
        string deviceCondition,
        string deviceBrand,
        string deviceModel,
        int deviceAge,
        decimal baseValue,
        DateTime validatedAt)
    {
        QuoteId = quoteId;
        IsValid = isValid;
        ValidationMessage = validationMessage;
        DeviceType = deviceType;
        DeviceCondition = deviceCondition;
        DeviceBrand = deviceBrand;
        DeviceModel = deviceModel;
        DeviceAge = deviceAge;
        BaseValue = baseValue;
        ValidatedAt = validatedAt;
    }
}