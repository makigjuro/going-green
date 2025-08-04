using GoingGreen.Shipping.Application.Domain.Events;
using GoingGreen.Shipping.Application.Domain.ValueObjects;

namespace GoingGreen.Shipping.Application.Domain.Aggregates;

public class Shipment
{
    public Guid Id { get; private set; }
    public Guid QuoteId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string DeviceType { get; private set; } = string.Empty;
    public string DeviceBrand { get; private set; } = string.Empty;
    public string DeviceModel { get; private set; } = string.Empty;
    public decimal QuoteValue { get; private set; }
    public ShippingStatus Status { get; private set; }
    public string? TrackingNumber { get; private set; }
    public string? ShippingCarrier { get; private set; }
    public string? ShippingService { get; private set; }
    public string? LabelUrl { get; private set; }
    public decimal ShippingCost { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? LabelGeneratedAt { get; private set; }
    public DateTime? ServiceNotifiedAt { get; private set; }
    public DateTime? EstimatedDeliveryDate { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    private Shipment() { }

    public static Shipment RequestShipping(
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
        var shipment = new Shipment
        {
            Id = shipmentId,
            QuoteId = quoteId,
            CustomerId = customerId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            DeviceType = deviceType,
            DeviceBrand = deviceBrand,
            DeviceModel = deviceModel,
            QuoteValue = quoteValue,
            Status = ShippingStatus.Requested,
            RequestedAt = requestedAt
        };

        return shipment;
    }

    public void OnLabelGenerated(
        string trackingNumber,
        string shippingCarrier,
        string shippingService,
        string labelUrl,
        decimal shippingCost,
        DateTime estimatedDeliveryDate,
        DateTime generatedAt)
    {
        if (Status != ShippingStatus.Requested)
            throw new InvalidOperationException($"Cannot generate label for shipment in status {Status}");

        TrackingNumber = trackingNumber;
        ShippingCarrier = shippingCarrier;
        ShippingService = shippingService;
        LabelUrl = labelUrl;
        ShippingCost = shippingCost;
        EstimatedDeliveryDate = estimatedDeliveryDate;
        LabelGeneratedAt = generatedAt;
        Status = ShippingStatus.LabelGenerated;
    }

    public void OnServiceNotified(
        string notificationId,
        string notificationStatus,
        DateTime notifiedAt)
    {
        if (Status != ShippingStatus.LabelGenerated)
            throw new InvalidOperationException($"Cannot notify service for shipment in status {Status}");

        ServiceNotifiedAt = notifiedAt;
        Status = ShippingStatus.ServiceNotified;
    }

    public void OnTrackingInfoUpdated(
        string status,
        string statusDescription,
        string location,
        DateTime updatedAt,
        DateTime? estimatedDeliveryDate = null)
    {
        if (Status == ShippingStatus.Delivered || Status == ShippingStatus.Cancelled)
            throw new InvalidOperationException($"Cannot update tracking for shipment in status {Status}");

        var newStatus = status.ToLowerInvariant() switch
        {
            "in_transit" or "in transit" => ShippingStatus.InTransit,
            "out_for_delivery" or "out for delivery" => ShippingStatus.OutForDelivery,
            "delivered" => ShippingStatus.Delivered,
            "returned" => ShippingStatus.Returned,
            "lost" => ShippingStatus.Lost,
            _ => Status
        };

        Status = newStatus;

        if (estimatedDeliveryDate.HasValue)
        {
            EstimatedDeliveryDate = estimatedDeliveryDate;
        }

        if (newStatus == ShippingStatus.Delivered)
        {
            DeliveredAt = updatedAt;
        }
    }

    public void OnPackageDelivered(
        DateTime deliveredAt,
        string deliveredTo,
        string deliveryLocation)
    {
        if (Status == ShippingStatus.Cancelled)
            throw new InvalidOperationException("Cannot deliver a cancelled shipment");

        Status = ShippingStatus.Delivered;
        DeliveredAt = deliveredAt;
    }

    public void Cancel(string cancellationReason, DateTime cancelledAt)
    {
        if (Status == ShippingStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel a delivered shipment");

        Status = ShippingStatus.Cancelled;
        CancellationReason = cancellationReason;
        CancelledAt = cancelledAt;
    }
}