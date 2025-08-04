namespace GoingGreen.Shipping.Application.Domain.Events;

public class ShippingServiceNotified
{
    public Guid ShipmentId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
    public string ShippingCarrier { get; init; } = string.Empty;
    public string NotificationId { get; init; } = string.Empty;
    public string NotificationStatus { get; init; } = string.Empty;
    public DateTime NotifiedAt { get; init; }

    public ShippingServiceNotified() { }

    public ShippingServiceNotified(
        Guid shipmentId,
        string trackingNumber,
        string shippingCarrier,
        string notificationId,
        string notificationStatus,
        DateTime notifiedAt)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        ShippingCarrier = shippingCarrier;
        NotificationId = notificationId;
        NotificationStatus = notificationStatus;
        NotifiedAt = notifiedAt;
    }
}