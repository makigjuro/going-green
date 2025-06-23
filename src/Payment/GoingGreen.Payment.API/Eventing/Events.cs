namespace Payment.API;

public record CustomerAcceptedFinalQuote(Guid AssessmentId);
public record PayoutInitiated(Guid PayoutId, Guid CustomerId, decimal Amount);
public record PayoutCompleted(Guid PayoutId, string TransactionId);
