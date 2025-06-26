namespace Quote.API.Application.Queries;

using Marten;
using Quote.API.Domain;
using GoingGreen.CQRS;

public record GetQuote(Guid Id)
{
    public class Handler : IQueryHandler<GetQuote, QuoteAggregate?>
    {
        private readonly IDocumentStore _store;

        public Handler(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<QuoteAggregate?> HandleAsync(GetQuote query, CancellationToken cancellationToken = default)
        {
            using var session = _store.QuerySession();
            return await session.Events.AggregateStreamAsync<QuoteAggregate>(query.Id, token: cancellationToken);
        }
    }
}
