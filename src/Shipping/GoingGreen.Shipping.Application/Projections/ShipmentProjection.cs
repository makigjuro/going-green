using GoingGreen.Shipping.Application.Domain.Events;
using GoingGreen.Shipping.Application.Domain.ValueObjects;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace GoingGreen.Shipping.Application.Projections;

public class ShipmentProjection
{
    public Guid Id { get; set; }
    public Guid QuoteId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceBrand { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public decimal QuoteValue { get; set; }
    public ShippingStatus Status { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShippingCarrier { get; set; }
    public string? ShippingService { get; set; }
    public string? LabelUrl { get; set; }
    public decimal ShippingCost { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? LabelGeneratedAt { get; set; }
    public DateTime? ServiceNotifiedAt { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? CurrentLocation { get; set; }
    public string? LastStatusDescription { get; set; }
    public DateTime? LastTrackingUpdate { get; set; }
}

public class ShipmentProjectionBuilder : SingleStreamProjection<ShipmentProjection, Guid>
{
    public ShipmentProjection Create(ShippingRequested @event)
    {
        return new ShipmentProjection
        {
            Id = @event.ShipmentId,
            QuoteId = @event.QuoteId,
            CustomerId = @event.CustomerId,
            CustomerName = @event.CustomerName,
            CustomerEmail = @event.CustomerEmail,
            DeviceType = @event.DeviceType,
            DeviceBrand = @event.DeviceBrand,
            DeviceModel = @event.DeviceModel,
            QuoteValue = @event.QuoteValue,
            Status = ShippingStatus.Requested,
            RequestedAt = @event.RequestedAt
        };
    }

    public void Apply(ShippingLabelGenerated @event, ShipmentProjection projection)
    {
        projection.Status = ShippingStatus.LabelGenerated;
        projection.TrackingNumber = @event.TrackingNumber;
        projection.ShippingCarrier = @event.ShippingCarrier;
        projection.ShippingService = @event.ShippingService;
        projection.LabelUrl = @event.LabelUrl;
        projection.ShippingCost = @event.ShippingCost;
        projection.EstimatedDeliveryDate = @event.EstimatedDeliveryDate;
        projection.LabelGeneratedAt = @event.GeneratedAt;
    }

    public void Apply(ShippingServiceNotified @event, ShipmentProjection projection)
    {
        projection.Status = ShippingStatus.ServiceNotified;
        projection.ServiceNotifiedAt = @event.NotifiedAt;
    }

    public void Apply(TrackingInfoUpdated @event, ShipmentProjection projection)
    {
        var newStatus = @event.Status.ToLowerInvariant() switch
        {
            "in_transit" or "in transit" => ShippingStatus.InTransit,
            "out_for_delivery" or "out for delivery" => ShippingStatus.OutForDelivery,
            "delivered" => ShippingStatus.Delivered,
            "returned" => ShippingStatus.Returned,
            "lost" => ShippingStatus.Lost,
            _ => projection.Status
        };

        projection.Status = newStatus;
        projection.CurrentLocation = @event.Location;
        projection.LastStatusDescription = @event.StatusDescription;
        projection.LastTrackingUpdate = @event.UpdatedAt;

        if (@event.EstimatedDeliveryDate.HasValue)
        {
            projection.EstimatedDeliveryDate = @event.EstimatedDeliveryDate;
        }

        if (newStatus == ShippingStatus.Delivered)
        {
            projection.DeliveredAt = @event.UpdatedAt;
        }
    }

    public void Apply(PackageDelivered @event, ShipmentProjection projection)
    {
        projection.Status = ShippingStatus.Delivered;
        projection.DeliveredAt = @event.DeliveredAt;
        projection.CurrentLocation = @event.DeliveryLocation;
        projection.LastStatusDescription = $"Delivered to {@event.DeliveredTo}";
        projection.LastTrackingUpdate = @event.DeliveredAt;
    }

    public void Apply(ShippingCancelled @event, ShipmentProjection projection)
    {
        projection.Status = ShippingStatus.Cancelled;
        projection.CancelledAt = @event.CancelledAt;
        projection.CancellationReason = @event.CancellationReason;
    }
}