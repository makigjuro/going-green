namespace GoingGreen.Shipping.Application.Domain.Events;

public class TrackingInfoUpdated
{
    public Guid ShipmentId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string StatusDescription { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; }
    public DateTime? EstimatedDeliveryDate { get; init; }

    public TrackingInfoUpdated() { }

    public TrackingInfoUpdated(
        Guid shipmentId,
        string trackingNumber,
        string status,
        string statusDescription,
        string location,
        DateTime updatedAt,
        DateTime? estimatedDeliveryDate = null)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        Status = status;
        StatusDescription = statusDescription;
        Location = location;
        UpdatedAt = updatedAt;
        EstimatedDeliveryDate = estimatedDeliveryDate;
    }
}