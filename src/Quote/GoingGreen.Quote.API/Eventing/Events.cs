namespace Quote.API;

public record QuoteRequested(Guid QuoteId, Guid DeviceId, decimal InitialValue, string CustomerInfo);
public record QuoteProvided(Guid QuoteId, decimal EstimatedValue);
