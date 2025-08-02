namespace GoingGreen.Quote.Application.Domain.Events;

public class QuoteAccepted
{
    public Guid QuoteId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime AcceptedAt { get; init; }

    public QuoteAccepted() { }

    public QuoteAccepted(
        Guid quoteId,
        Guid customerId,
        DateTime acceptedAt)
    {
        QuoteId = quoteId;
        CustomerId = customerId;
        AcceptedAt = acceptedAt;
    }
}