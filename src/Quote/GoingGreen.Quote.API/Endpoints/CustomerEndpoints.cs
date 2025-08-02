using GoingGreen.Quote.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Quote.API.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var customers = app.MapGroup("/customers").WithTags("Customers");

        customers.MapGet("/{customerId:guid}/quotes", async (
            Guid customerId,
            [FromServices] IQuoteService quoteService) =>
        {
            var customerQuotes = await quoteService.GetCustomerQuotesAsync(customerId);
            return customerQuotes is not null ? Results.Ok(customerQuotes) : Results.NotFound();
        })
        .WithName("GetCustomerQuotes")
        .WithOpenApi();
    }
}