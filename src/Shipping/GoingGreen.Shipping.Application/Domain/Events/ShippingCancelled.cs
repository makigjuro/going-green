namespace GoingGreen.Shipping.Application.Domain.Events;

public class ShippingCancelled
{
    public Guid ShipmentId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
    public string CancellationReason { get; init; } = string.Empty;
    public DateTime CancelledAt { get; init; }
    public decimal RefundAmount { get; init; }

    public ShippingCancelled() { }

    public ShippingCancelled(
        Guid shipmentId,
        string trackingNumber,
        string cancellationReason,
        DateTime cancelledAt,
        decimal refundAmount = 0)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        CancellationReason = cancellationReason;
        CancelledAt = cancelledAt;
        RefundAmount = refundAmount;
    }
}