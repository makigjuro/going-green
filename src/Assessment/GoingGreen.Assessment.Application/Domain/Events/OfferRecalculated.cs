namespace GoingGreen.Assessment.Application.Domain.Events;

public class OfferRecalculated
{
    public Guid AssessmentId { get; init; }
    public Guid QuoteId { get; init; }
    public decimal OriginalOffer { get; init; }
    public decimal NewOffer { get; init; }
    public decimal AdjustmentAmount { get; init; }
    public string AdjustmentReason { get; init; } = string.Empty;
    public List<string> ConditionMismatches { get; init; } = new();
    public DateTime RecalculatedAt { get; init; }
    public bool RequiresCustomerApproval { get; init; }

    public OfferRecalculated() { }

    public OfferRecalculated(
        Guid assessmentId,
        Guid quoteId,
        decimal originalOffer,
        decimal newOffer,
        decimal adjustmentAmount,
        string adjustmentReason,
        List<string> conditionMismatches,
        DateTime recalculatedAt,
        bool requiresCustomerApproval)
    {
        AssessmentId = assessmentId;
        QuoteId = quoteId;
        OriginalOffer = originalOffer;
        NewOffer = newOffer;
        AdjustmentAmount = adjustmentAmount;
        AdjustmentReason = adjustmentReason;
        ConditionMismatches = conditionMismatches;
        RecalculatedAt = recalculatedAt;
        RequiresCustomerApproval = requiresCustomerApproval;
    }
}