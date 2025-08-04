using GoingGreen.Shipping.Application.Domain.Events;
using GoingGreen.Shipping.Application.Domain.ValueObjects;
using Marten.Events.Projections;

namespace GoingGreen.Shipping.Application.Projections;

public class CustomerShipmentsProjection
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public List<CustomerShipmentItem> Shipments { get; set; } = new();
    public int TotalShipments { get; set; }
    public int DeliveredShipments { get; set; }
    public int ActiveShipments { get; set; }
    public decimal TotalShippingCost { get; set; }
}

public class CustomerShipmentItem
{
    public Guid ShipmentId { get; set; }
    public Guid QuoteId { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceBrand { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public decimal QuoteValue { get; set; }
    public ShippingStatus Status { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShippingCarrier { get; set; }
    public decimal ShippingCost { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

public class CustomerShipmentsProjectionBuilder : MultiStreamProjection<CustomerShipmentsProjection, Guid>
{
    public CustomerShipmentsProjectionBuilder()
    {
        Identity<ShippingRequested>(x => x.CustomerId);
        Identity<ShippingLabelGenerated>(x => x.ShipmentId); // This will need custom handling
        Identity<PackageDelivered>(x => x.ShipmentId); // This will need custom handling
    }

    public CustomerShipmentsProjection Create(ShippingRequested @event)
    {
        return new CustomerShipmentsProjection
        {
            CustomerId = @event.CustomerId,
            CustomerName = @event.CustomerName,
            CustomerEmail = @event.CustomerEmail,
            Shipments = new List<CustomerShipmentItem>
            {
                new()
                {
                    ShipmentId = @event.ShipmentId,
                    QuoteId = @event.QuoteId,
                    DeviceType = @event.DeviceType,
                    DeviceBrand = @event.DeviceBrand,
                    DeviceModel = @event.DeviceModel,
                    QuoteValue = @event.QuoteValue,
                    Status = ShippingStatus.Requested,
                    RequestedAt = @event.RequestedAt
                }
            },
            TotalShipments = 1,
            ActiveShipments = 1
        };
    }

    public void Apply(ShippingRequested @event, CustomerShipmentsProjection projection)
    {
        var existingShipment = projection.Shipments.FirstOrDefault(s => s.ShipmentId == @event.ShipmentId);
        if (existingShipment == null)
        {
            projection.Shipments.Add(new CustomerShipmentItem
            {
                ShipmentId = @event.ShipmentId,
                QuoteId = @event.QuoteId,
                DeviceType = @event.DeviceType,
                DeviceBrand = @event.DeviceBrand,
                DeviceModel = @event.DeviceModel,
                QuoteValue = @event.QuoteValue,
                Status = ShippingStatus.Requested,
                RequestedAt = @event.RequestedAt
            });
            projection.TotalShipments++;
            projection.ActiveShipments++;
        }
    }

    public void Apply(ShippingLabelGenerated @event, CustomerShipmentsProjection projection)
    {
        var shipment = projection.Shipments.FirstOrDefault(s => s.ShipmentId == @event.ShipmentId);
        if (shipment != null)
        {
            shipment.Status = ShippingStatus.LabelGenerated;
            shipment.TrackingNumber = @event.TrackingNumber;
            shipment.ShippingCarrier = @event.ShippingCarrier;
            shipment.ShippingCost = @event.ShippingCost;
            shipment.EstimatedDeliveryDate = @event.EstimatedDeliveryDate;
            projection.TotalShippingCost += @event.ShippingCost;
        }
    }

    public void Apply(PackageDelivered @event, CustomerShipmentsProjection projection)
    {
        var shipment = projection.Shipments.FirstOrDefault(s => s.ShipmentId == @event.ShipmentId);
        if (shipment != null)
        {
            shipment.Status = ShippingStatus.Delivered;
            shipment.DeliveredAt = @event.DeliveredAt;
            projection.DeliveredShipments++;
            projection.ActiveShipments--;
        }
    }

    public void Apply(ShippingCancelled @event, CustomerShipmentsProjection projection)
    {
        var shipment = projection.Shipments.FirstOrDefault(s => s.ShipmentId == @event.ShipmentId);
        if (shipment != null)
        {
            shipment.Status = ShippingStatus.Cancelled;
            projection.ActiveShipments--;
        }
    }
}