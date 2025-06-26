namespace Quote.API.Application.Commands;

using Marten;
using Quote.API.Domain;
using Quote.API.Domain.Events;
using Quote.API.Infrastructure.Messaging;

public record CreateQuote(Guid DeviceId, decimal InitialValue, string CustomerInfo)
{
    public class Handler
    {
        private readonly IDocumentStore _store;
        private readonly IEventPublisher _publisher;

        public Handler(IDocumentStore store, IEventPublisher publisher)
        {
            _store = store;
            _publisher = publisher;
        }

        public async Task<Guid> HandleAsync(CreateQuote command, CancellationToken cancellationToken = default)
        {
            var quoteId = Guid.NewGuid();
            var @event = new QuoteRequested(quoteId, command.DeviceId, command.InitialValue, command.CustomerInfo);
            using var session = _store.LightweightSession();
            session.Events.StartStream<QuoteAggregate>(quoteId, @event);
            await session.SaveChangesAsync(cancellationToken);
            await _publisher.PublishAsync(@event, cancellationToken);
            return quoteId;
        }
    }
}
