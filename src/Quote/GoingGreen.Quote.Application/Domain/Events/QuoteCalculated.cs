namespace GoingGreen.Quote.Application.Domain.Events;

public class QuoteCalculated
{
    public Guid QuoteId { get; init; }
    public decimal EstimatedValue { get; init; }
    public string QuoteReason { get; init; } = string.Empty;
    public DateTime CalculatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }

    public QuoteCalculated() { }

    public QuoteCalculated(
        Guid quoteId,
        decimal estimatedValue,
        string quoteReason,
        DateTime calculatedAt,
        DateTime expiresAt)
    {
        QuoteId = quoteId;
        EstimatedValue = estimatedValue;
        QuoteReason = quoteReason;
        CalculatedAt = calculatedAt;
        ExpiresAt = expiresAt;
    }
}