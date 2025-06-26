using Quote.API.Domain.Events;

namespace Quote.API.Domain;

public enum QuoteStatus
{
    Requested,
    Provided,
    Accepted
}

public class QuoteAggregate
{
    public Guid Id { get; private set; }
    public Guid DeviceId { get; private set; }
    public decimal InitialValue { get; private set; }
    public decimal? EstimatedValue { get; private set; }
    public string CustomerInfo { get; private set; } = string.Empty;
    public QuoteStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Empty constructor required for Marten
    public QuoteAggregate() { }

    public void Apply(QuoteRequested e)
    {
        Id = e.QuoteId;
        DeviceId = e.DeviceId;
        InitialValue = e.InitialValue;
        CustomerInfo = e.CustomerInfo;
        Status = QuoteStatus.Requested;
        CreatedAt = DateTime.UtcNow;
    }

    public void Apply(QuoteProvided e)
    {
        EstimatedValue = e.EstimatedValue;
        Status = QuoteStatus.Provided;
    }
}
