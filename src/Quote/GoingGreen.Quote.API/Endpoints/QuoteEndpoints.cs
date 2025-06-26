using Quote.API.Application.Commands;
using Quote.API.Application.Queries;

namespace Quote.API.Endpoints;

public static class QuoteEndpoints
{
    public static IEndpointRouteBuilder MapQuoteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/quotes");

        group.MapPost("/", async (CreateQuote command, CreateQuote.Handler createHandler, GetQuote.Handler getHandler, CancellationToken ct) =>
        {
            var id = await createHandler.HandleAsync(command, ct);
            var quote = await getHandler.HandleAsync(new GetQuote(id), ct);
            return Results.Created($"/quotes/{id}", quote);
        });

        group.MapPost("/{id:guid}/provide", async (Guid id, ProvideQuote request, ProvideQuote.Handler provideHandler, GetQuote.Handler getHandler, CancellationToken ct) =>
        {
            await provideHandler.HandleAsync(request with { QuoteId = id }, ct);
            var quote = await getHandler.HandleAsync(new GetQuote(id), ct);
            return Results.Ok(quote);
        });

        group.MapGet("/{id:guid}", async (Guid id, GetQuote.Handler handler, CancellationToken ct) =>
        {
            var quote = await handler.HandleAsync(new GetQuote(id), ct);
            return quote is null ? Results.NotFound() : Results.Ok(quote);
        });

        return app;
    }
}
