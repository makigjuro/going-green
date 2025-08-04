namespace GoingGreen.Quote.Application.Domain.ValueObjects;

public enum QuoteStatus
{
    Requested,
    DeviceValidationPending,
    DeviceValidated,
    Calculated,
    Rejected,
    Accepted,
    Expired
}