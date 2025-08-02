using GoingGreen.Quote.Application.Domain.Events;
using GoingGreen.Quote.Application.Domain.ValueObjects;
using Marten.Events.Projections;

namespace GoingGreen.Quote.Application.Projections;

public class CustomerQuotesProjection
{
    public Guid CustomerId { get; set; }
    public List<CustomerQuoteItem> Quotes { get; set; } = new();
    public int TotalQuotes { get; set; }
    public int AcceptedQuotes { get; set; }
    public decimal TotalQuoteValue { get; set; }
}

public class CustomerQuoteItem
{
    public Guid QuoteId { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceBrand { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public QuoteStatus Status { get; set; }
    public decimal? EstimatedValue { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
}

public class CustomerQuotesProjectionBuilder : MultiStreamProjection<CustomerQuotesProjection, Guid>
{
    public CustomerQuotesProjectionBuilder()
    {
        Identity<QuoteRequested>(x => x.CustomerId);
        Identity<QuoteCalculated>(x => x.QuoteId); // This will need custom handling
        Identity<QuoteAccepted>(x => x.CustomerId);
    }

    public CustomerQuotesProjection Create(QuoteRequested @event)
    {
        return new CustomerQuotesProjection
        {
            CustomerId = @event.CustomerId,
            Quotes = new List<CustomerQuoteItem>
            {
                new()
                {
                    QuoteId = @event.QuoteId,
                    DeviceType = @event.DeviceType,
                    DeviceBrand = @event.DeviceBrand,
                    DeviceModel = @event.DeviceModel,
                    Status = QuoteStatus.Requested,
                    RequestedAt = @event.RequestedAt
                }
            },
            TotalQuotes = 1
        };
    }

    public void Apply(QuoteRequested @event, CustomerQuotesProjection projection)
    {
        var existingQuote = projection.Quotes.FirstOrDefault(q => q.QuoteId == @event.QuoteId);
        if (existingQuote == null)
        {
            projection.Quotes.Add(new CustomerQuoteItem
            {
                QuoteId = @event.QuoteId,
                DeviceType = @event.DeviceType,
                DeviceBrand = @event.DeviceBrand,
                DeviceModel = @event.DeviceModel,
                Status = QuoteStatus.Requested,
                RequestedAt = @event.RequestedAt
            });
            projection.TotalQuotes++;
        }
    }

    public void Apply(QuoteCalculated @event, CustomerQuotesProjection projection)
    {
        var quote = projection.Quotes.FirstOrDefault(q => q.QuoteId == @event.QuoteId);
        if (quote != null)
        {
            quote.Status = QuoteStatus.Calculated;
            quote.EstimatedValue = @event.EstimatedValue;
        }
    }

    public void Apply(QuoteAccepted @event, CustomerQuotesProjection projection)
    {
        var quote = projection.Quotes.FirstOrDefault(q => q.QuoteId == @event.QuoteId);
        if (quote != null)
        {
            quote.Status = QuoteStatus.Accepted;
            quote.AcceptedAt = @event.AcceptedAt;
            projection.AcceptedQuotes++;
            projection.TotalQuoteValue += quote.EstimatedValue ?? 0;
        }
    }
}