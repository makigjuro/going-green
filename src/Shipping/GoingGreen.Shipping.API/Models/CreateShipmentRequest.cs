namespace Shipping.API.Models;

public record CreateShipmentRequest(
    Guid QuoteId,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string DeviceType,
    string DeviceBrand,
    string DeviceModel,
    decimal QuoteValue);