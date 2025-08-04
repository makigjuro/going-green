namespace GoingGreen.Shipping.Application.Domain.Events;

public class ShippingRequested
{
    public Guid ShipmentId { get; init; }
    public Guid QuoteId { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public string DeviceType { get; init; } = string.Empty;
    public string DeviceBrand { get; init; } = string.Empty;
    public string DeviceModel { get; init; } = string.Empty;
    public decimal QuoteValue { get; init; }
    public DateTime RequestedAt { get; init; }

    public ShippingRequested() { }

    public ShippingRequested(
        Guid shipmentId,
        Guid quoteId,
        Guid customerId,
        string customerName,
        string customerEmail,
        string deviceType,
        string deviceBrand,
        string deviceModel,
        decimal quoteValue,
        DateTime requestedAt)
    {
        ShipmentId = shipmentId;
        QuoteId = quoteId;
        CustomerId = customerId;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        DeviceType = deviceType;
        DeviceBrand = deviceBrand;
        DeviceModel = deviceModel;
        QuoteValue = quoteValue;
        RequestedAt = requestedAt;
    }
}