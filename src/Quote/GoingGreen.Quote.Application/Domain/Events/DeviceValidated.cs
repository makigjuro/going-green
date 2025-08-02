namespace GoingGreen.Quote.Application.Domain.Events;

public class DeviceValidated
{
    public Guid QuoteId { get; init; }
    public bool IsValid { get; init; }
    public string ValidationMessage { get; init; } = string.Empty;
    public DateTime ValidatedAt { get; init; }

    public DeviceValidated() { }

    public DeviceValidated(
        Guid quoteId,
        bool isValid,
        string validationMessage,
        DateTime validatedAt)
    {
        QuoteId = quoteId;
        IsValid = isValid;
        ValidationMessage = validationMessage;
        ValidatedAt = validatedAt;
    }
}