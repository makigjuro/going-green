namespace GoingGreen.Quote.Application.Domain.Events;

public class QuoteExpired
{
    public Guid QuoteId { get; init; }
    public DateTime ExpiredAt { get; init; }

    public QuoteExpired() { }

    public QuoteExpired(
        Guid quoteId,
        DateTime expiredAt)
    {
        QuoteId = quoteId;
        ExpiredAt = expiredAt;
    }
}