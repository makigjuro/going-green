using GoingGreen.Quote.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Quote.API.Models;
using Quote.API.IntegrationEvents;
using Quote.API;

namespace Quote.API.Endpoints;

public static class QuoteEndpoints
{
    public static void MapQuoteEndpoints(this IEndpointRouteBuilder app)
    {
        var quotes = app.MapGroup("/quotes").WithTags("Quotes");

        quotes.MapPost("/", async (
            [FromBody] QuoteRequest request,
            [FromServices] IQuoteService quoteService) =>
        {
            var quoteId = await quoteService.RequestQuoteAsync(
                request.CustomerId,
                request.DeviceType,
                request.DeviceCondition,
                request.DeviceBrand,
                request.DeviceModel,
                request.DeviceAge);

            return Results.Created($"/quotes/{quoteId}", new { QuoteId = quoteId });
        })
        .WithName("RequestQuote")
        .WithOpenApi();

        quotes.MapGet("/{quoteId:guid}", async (
            Guid quoteId,
            [FromServices] IQuoteService quoteService) =>
        {
            var quote = await quoteService.GetQuoteAsync(quoteId);
            return quote is not null ? Results.Ok(quote) : Results.NotFound();
        })
        .WithName("GetQuote")
        .WithOpenApi();

        quotes.MapPost("/{quoteId:guid}/accept", async (
            Guid quoteId,
            [FromServices] IQuoteService quoteService,
            [FromServices] IEventPublisher eventPublisher) =>
        {
            try
            {
                await quoteService.AcceptQuoteAsync(quoteId);

                // Get quote details for integration event
                var quote = await quoteService.GetQuoteAsync(quoteId);
                if (quote != null)
                {
                    // Publish integration event for other services (like Shipping)
                    var integrationEvent = new QuoteAcceptedIntegrationEvent(
                        quote.Id,
                        quote.CustomerId,
                        "Customer Name", // TODO: Get from Customer service
                        "customer@example.com", // TODO: Get from Customer service
                        quote.DeviceType,
                        quote.DeviceBrand,
                        quote.DeviceModel,
                        quote.EstimatedValue ?? 0,
                        DateTime.UtcNow);

                    await eventPublisher.PublishAsync(integrationEvent);
                }

                return Results.Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("AcceptQuote")
        .WithOpenApi();

        quotes.MapPost("/{quoteId:guid}/reject", async (
            Guid quoteId,
            [FromBody] RejectQuoteRequest request,
            [FromServices] IQuoteService quoteService) =>
        {
            await quoteService.RejectQuoteAsync(quoteId, request.Reason);
            return Results.Ok();
        })
        .WithName("RejectQuote")
        .WithOpenApi();
    }
}