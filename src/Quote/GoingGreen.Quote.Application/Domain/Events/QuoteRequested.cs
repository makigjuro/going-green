namespace GoingGreen.Quote.Application.Domain.Events;

public class QuoteRequested
{
    public Guid QuoteId { get; init; }
    public Guid CustomerId { get; init; }
    public string DeviceType { get; init; } = string.Empty;
    public string DeviceCondition { get; init; } = string.Empty;
    public string DeviceBrand { get; init; } = string.Empty;
    public string DeviceModel { get; init; } = string.Empty;
    public int DeviceAge { get; init; }
    public DateTime RequestedAt { get; init; }

    public QuoteRequested() { }

    public QuoteRequested(
        Guid quoteId,
        Guid customerId,
        string deviceType,
        string deviceCondition,
        string deviceBrand,
        string deviceModel,
        int deviceAge,
        DateTime requestedAt)
    {
        QuoteId = quoteId;
        CustomerId = customerId;
        DeviceType = deviceType;
        DeviceCondition = deviceCondition;
        DeviceBrand = deviceBrand;
        DeviceModel = deviceModel;
        DeviceAge = deviceAge;
        RequestedAt = requestedAt;
    }
}