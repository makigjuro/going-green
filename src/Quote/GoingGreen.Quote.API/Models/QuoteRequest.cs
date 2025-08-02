namespace Quote.API.Models;

public record QuoteRequest(
    Guid CustomerId,
    string DeviceType,
    string DeviceCondition,
    string DeviceBrand,
    string DeviceModel,
    int DeviceAge);