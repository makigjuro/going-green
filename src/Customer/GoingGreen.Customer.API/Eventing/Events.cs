namespace Customer.API;

public record QuoteAdjustmentNotified(Guid AssessmentId, decimal AdjustedQuote);
public record CustomerAcceptedFinalQuote(Guid AssessmentId);
public record CustomerRejectedFinalQuote(Guid AssessmentId, string Reason);
