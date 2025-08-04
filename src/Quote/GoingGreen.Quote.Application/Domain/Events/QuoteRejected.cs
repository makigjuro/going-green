namespace GoingGreen.Quote.Application.Domain.Events;

public class QuoteRejected
{
    public Guid QuoteId { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
    public DateTime RejectedAt { get; init; }

    public QuoteRejected() { }

    public QuoteRejected(
        Guid quoteId,
        string rejectionReason,
        DateTime rejectedAt)
    {
        QuoteId = quoteId;
        RejectionReason = rejectionReason;
        RejectedAt = rejectedAt;
    }
}