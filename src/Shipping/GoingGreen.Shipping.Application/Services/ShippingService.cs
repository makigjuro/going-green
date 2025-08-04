using GoingGreen.Shipping.Application.Domain.Aggregates;
using GoingGreen.Shipping.Application.Domain.Events;
using GoingGreen.Shipping.Application.Projections;
using Marten;

namespace GoingGreen.Shipping.Application.Services;

public interface IShippingService
{
    Task<Guid> RequestShippingAsync(Guid quoteId, Guid customerId, string customerName, 
        string customerEmail, string deviceType, string deviceBrand, string deviceModel, 
        decimal quoteValue, CancellationToken cancellationToken = default);
    Task<ShipmentProjection?> GetShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<ShipmentProjection?> GetShipmentByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task<CustomerShipmentsProjection?> GetCustomerShipmentsAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task GenerateShippingLabelAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task UpdateTrackingInfoAsync(Guid shipmentId, string status, string statusDescription, 
        string location, DateTime? estimatedDeliveryDate = null, CancellationToken cancellationToken = default);
    Task MarkAsDeliveredAsync(Guid shipmentId, string deliveredTo, string deliveryLocation, 
        CancellationToken cancellationToken = default);
    Task CancelShippingAsync(Guid shipmentId, string reason, CancellationToken cancellationToken = default);
}

public class ShippingService : IShippingService
{
    private readonly IDocumentSession _session;
    private readonly IShippingLabelGenerator _labelGenerator;
    private readonly IShippingCarrierNotifier _carrierNotifier;

    public ShippingService(
        IDocumentSession session, 
        IShippingLabelGenerator labelGenerator,
        IShippingCarrierNotifier carrierNotifier)
    {
        _session = session;
        _labelGenerator = labelGenerator;
        _carrierNotifier = carrierNotifier;
    }

    public async Task<Guid> RequestShippingAsync(Guid quoteId, Guid customerId, string customerName, 
        string customerEmail, string deviceType, string deviceBrand, string deviceModel, 
        decimal quoteValue, CancellationToken cancellationToken = default)
    {
        var shipmentId = Guid.NewGuid();
        var requestedAt = DateTime.UtcNow;

        var shippingRequested = new ShippingRequested(
            shipmentId, quoteId, customerId, customerName, customerEmail,
            deviceType, deviceBrand, deviceModel, quoteValue, requestedAt);

        _session.Events.StartStream<Shipment>(shipmentId, shippingRequested);
        await _session.SaveChangesAsync(cancellationToken);

        return shipmentId;
    }

    public async Task<ShipmentProjection?> GetShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _session.Query<ShipmentProjection>()
            .FirstOrDefaultAsync(s => s.Id == shipmentId, cancellationToken);
    }

    public async Task<ShipmentProjection?> GetShipmentByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        return await _session.Query<ShipmentProjection>()
            .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber, cancellationToken);
    }

    public async Task<CustomerShipmentsProjection?> GetCustomerShipmentsAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _session.Query<CustomerShipmentsProjection>()
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
    }

    public async Task GenerateShippingLabelAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        var shipment = await GetShipmentAsync(shipmentId, cancellationToken);
        if (shipment == null)
            throw new InvalidOperationException($"Shipment {shipmentId} not found");

        // Generate label using external service
        var labelInfo = await _labelGenerator.GenerateLabelAsync(shipment);
        
        var labelGenerated = new ShippingLabelGenerated(
            shipmentId,
            labelInfo.TrackingNumber,
            labelInfo.Carrier,
            labelInfo.Service,
            labelInfo.LabelUrl,
            labelInfo.Cost,
            labelInfo.EstimatedDeliveryDate,
            DateTime.UtcNow);

        _session.Events.Append(shipmentId, labelGenerated);

        // Notify shipping service
        var notificationResult = await _carrierNotifier.NotifyCarrierAsync(labelInfo);
        
        var serviceNotified = new ShippingServiceNotified(
            shipmentId,
            labelInfo.TrackingNumber,
            labelInfo.Carrier,
            notificationResult.NotificationId,
            notificationResult.Status,
            DateTime.UtcNow);

        _session.Events.Append(shipmentId, serviceNotified);
        await _session.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTrackingInfoAsync(Guid shipmentId, string status, string statusDescription, 
        string location, DateTime? estimatedDeliveryDate = null, CancellationToken cancellationToken = default)
    {
        var shipment = await GetShipmentAsync(shipmentId, cancellationToken);
        if (shipment == null)
            throw new InvalidOperationException($"Shipment {shipmentId} not found");

        var trackingUpdated = new TrackingInfoUpdated(
            shipmentId,
            shipment.TrackingNumber ?? "",
            status,
            statusDescription,
            location,
            DateTime.UtcNow,
            estimatedDeliveryDate);

        _session.Events.Append(shipmentId, trackingUpdated);
        await _session.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsDeliveredAsync(Guid shipmentId, string deliveredTo, string deliveryLocation, 
        CancellationToken cancellationToken = default)
    {
        var shipment = await GetShipmentAsync(shipmentId, cancellationToken);
        if (shipment == null)
            throw new InvalidOperationException($"Shipment {shipmentId} not found");

        var packageDelivered = new PackageDelivered(
            shipmentId,
            shipment.TrackingNumber ?? "",
            DateTime.UtcNow,
            deliveredTo,
            deliveryLocation);

        _session.Events.Append(shipmentId, packageDelivered);
        await _session.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelShippingAsync(Guid shipmentId, string reason, CancellationToken cancellationToken = default)
    {
        var shipment = await GetShipmentAsync(shipmentId, cancellationToken);
        if (shipment == null)
            throw new InvalidOperationException($"Shipment {shipmentId} not found");

        var shippingCancelled = new ShippingCancelled(
            shipmentId,
            shipment.TrackingNumber ?? "",
            reason,
            DateTime.UtcNow,
            shipment.ShippingCost);

        _session.Events.Append(shipmentId, shippingCancelled);
        await _session.SaveChangesAsync(cancellationToken);
    }
}

// External service interfaces
public interface IShippingLabelGenerator
{
    Task<ShippingLabelInfo> GenerateLabelAsync(ShipmentProjection shipment);
}

public interface IShippingCarrierNotifier
{
    Task<CarrierNotificationResult> NotifyCarrierAsync(ShippingLabelInfo labelInfo);
}

public record ShippingLabelInfo(
    string TrackingNumber,
    string Carrier,
    string Service,
    string LabelUrl,
    decimal Cost,
    DateTime EstimatedDeliveryDate);

public record CarrierNotificationResult(
    string NotificationId,
    string Status);