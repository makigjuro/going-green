namespace Quote.API.IntegrationEvents;

public record QuoteAcceptedIntegrationEvent(
    Guid QuoteId,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string DeviceType,
    string DeviceBrand,
    string DeviceModel,
    decimal EstimatedValue,
    DateTime AcceptedAt);