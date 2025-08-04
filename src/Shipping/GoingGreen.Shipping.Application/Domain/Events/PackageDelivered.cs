namespace GoingGreen.Shipping.Application.Domain.Events;

public class PackageDelivered
{
    public Guid ShipmentId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
    public DateTime DeliveredAt { get; init; }
    public string DeliveredTo { get; init; } = string.Empty;
    public string DeliveryLocation { get; init; } = string.Empty;
    public string DeliverySignature { get; init; } = string.Empty;
    public string DeliveryPhotoUrl { get; init; } = string.Empty;

    public PackageDelivered() { }

    public PackageDelivered(
        Guid shipmentId,
        string trackingNumber,
        DateTime deliveredAt,
        string deliveredTo,
        string deliveryLocation,
        string deliverySignature = "",
        string deliveryPhotoUrl = "")
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        DeliveredAt = deliveredAt;
        DeliveredTo = deliveredTo;
        DeliveryLocation = deliveryLocation;
        DeliverySignature = deliverySignature;
        DeliveryPhotoUrl = deliveryPhotoUrl;
    }
}