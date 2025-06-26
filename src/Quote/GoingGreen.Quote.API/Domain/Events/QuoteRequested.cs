namespace Quote.API.Domain.Events;

public record QuoteRequested(Guid QuoteId, Guid DeviceId, decimal InitialValue, string CustomerInfo);
