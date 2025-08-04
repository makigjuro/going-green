using GoingGreen.Quote.Application.Domain.Aggregates;
using GoingGreen.Quote.Application.Domain.Events;
using GoingGreen.Quote.Application.Projections;
using Marten;

namespace GoingGreen.Quote.Application.Services;

public interface IQuoteService
{
    Task<Guid> RequestQuoteAsync(Guid customerId, string deviceType, string deviceCondition, 
        string deviceBrand, string deviceModel, int deviceAge, CancellationToken cancellationToken = default);
    Task<QuoteProjection?> GetQuoteAsync(Guid quoteId, CancellationToken cancellationToken = default);
    Task<CustomerQuotesProjection?> GetCustomerQuotesAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AcceptQuoteAsync(Guid quoteId, CancellationToken cancellationToken = default);
    Task RejectQuoteAsync(Guid quoteId, string reason, CancellationToken cancellationToken = default);
}

public class QuoteService : IQuoteService
{
    private readonly IDocumentSession _session;

    public QuoteService(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<Guid> RequestQuoteAsync(Guid customerId, string deviceType, string deviceCondition, 
        string deviceBrand, string deviceModel, int deviceAge, CancellationToken cancellationToken = default)
    {
        var quoteId = Guid.NewGuid();
        var requestedAt = DateTime.UtcNow;

        var quoteRequested = new QuoteRequested(
            quoteId, customerId, deviceType, deviceCondition, 
            deviceBrand, deviceModel, deviceAge, requestedAt);

        _session.Events.StartStream<Domain.Aggregates.Quote>(quoteId, quoteRequested);
        await _session.SaveChangesAsync(cancellationToken);

        return quoteId;
    }

    public async Task<QuoteProjection?> GetQuoteAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        return await _session.Query<QuoteProjection>()
            .FirstOrDefaultAsync(q => q.Id == quoteId, cancellationToken);
    }

    public async Task<CustomerQuotesProjection?> GetCustomerQuotesAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _session.Query<CustomerQuotesProjection>()
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
    }

    public async Task AcceptQuoteAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        var events = await _session.Events.FetchStreamAsync(quoteId, token: cancellationToken);
        var quote = Domain.Aggregates.Quote.RequestQuote(Guid.Empty, Guid.Empty, "", "", "", "", 0, DateTime.MinValue);
        
        // Apply events to rebuild aggregate state
        foreach (var @event in events)
        {
            switch (@event.Data)
            {
                case QuoteRequested requested:
                    quote = Domain.Aggregates.Quote.RequestQuote(
                        requested.QuoteId, requested.CustomerId, requested.DeviceType, 
                        requested.DeviceCondition, requested.DeviceBrand, requested.DeviceModel, 
                        requested.DeviceAge, requested.RequestedAt);
                    break;
                case DeviceValidated validated:
                    quote.OnDeviceValidated(validated.IsValid, validated.ValidationMessage, 0, validated.ValidatedAt);
                    break;
            }
        }

        var acceptedAt = DateTime.UtcNow;
        quote.Accept(acceptedAt);

        var quoteAccepted = new QuoteAccepted(quoteId, quote.CustomerId, acceptedAt);
        _session.Events.Append(quoteId, quoteAccepted);
        await _session.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectQuoteAsync(Guid quoteId, string reason, CancellationToken cancellationToken = default)
    {
        var rejectedAt = DateTime.UtcNow;
        var quoteRejected = new QuoteRejected(quoteId, reason, rejectedAt);
        
        _session.Events.Append(quoteId, quoteRejected);
        await _session.SaveChangesAsync(cancellationToken);
    }
}