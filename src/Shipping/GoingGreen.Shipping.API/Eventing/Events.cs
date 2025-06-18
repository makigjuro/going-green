namespace Shipping.API;

public record QuoteAccepted(Guid QuoteId);
public record ShippingLabelGenerated(Guid ShipmentId, string LabelUrl, string TrackingId);
public record PackageShipped(Guid ShipmentId, DateTime ShippedAt);
