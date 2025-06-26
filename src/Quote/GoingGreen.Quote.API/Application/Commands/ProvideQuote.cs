namespace Quote.API.Application.Commands;

using Marten;
using Quote.API.Domain.Events;
using Quote.API.Infrastructure.Messaging;

public record ProvideQuote(Guid QuoteId, decimal EstimatedValue)
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

        public async Task HandleAsync(ProvideQuote command, CancellationToken cancellationToken = default)
        {
            using var session = _store.LightweightSession();
            var @event = new QuoteProvided(command.QuoteId, command.EstimatedValue);
            session.Events.Append(command.QuoteId, @event);
            await session.SaveChangesAsync(cancellationToken);
            await _publisher.PublishAsync(@event, cancellationToken);
        }
    }
}
