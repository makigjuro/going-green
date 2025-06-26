namespace Quote.API.Domain.Events;

public record QuoteProvided(Guid QuoteId, decimal EstimatedValue);
