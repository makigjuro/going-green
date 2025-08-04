namespace Shipping.API.Models;

public record UpdateTrackingRequest(
    string Status,
    string StatusDescription,
    string Location,
    DateTime? EstimatedDeliveryDate = null);