namespace GoingGreen.Shipping.Application.Domain.Events;

public class ShippingLabelGenerated
{
    public Guid ShipmentId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
    public string ShippingCarrier { get; init; } = string.Empty;
    public string ShippingService { get; init; } = string.Empty;
    public string LabelUrl { get; init; } = string.Empty;
    public decimal ShippingCost { get; init; }
    public DateTime EstimatedDeliveryDate { get; init; }
    public DateTime GeneratedAt { get; init; }

    public ShippingLabelGenerated() { }

    public ShippingLabelGenerated(
        Guid shipmentId,
        string trackingNumber,
        string shippingCarrier,
        string shippingService,
        string labelUrl,
        decimal shippingCost,
        DateTime estimatedDeliveryDate,
        DateTime generatedAt)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        ShippingCarrier = shippingCarrier;
        ShippingService = shippingService;
        LabelUrl = labelUrl;
        ShippingCost = shippingCost;
        EstimatedDeliveryDate = estimatedDeliveryDate;
        GeneratedAt = generatedAt;
    }
}