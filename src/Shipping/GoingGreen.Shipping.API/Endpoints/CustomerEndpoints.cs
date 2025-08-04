using GoingGreen.Shipping.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Shipping.API.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var customers = app.MapGroup("/customers").WithTags("Customers");

        customers.MapGet("/{customerId:guid}/shipments", async (
            Guid customerId,
            [FromServices] IShippingService shippingService) =>
        {
            var customerShipments = await shippingService.GetCustomerShipmentsAsync(customerId);
            return customerShipments is not null ? Results.Ok(customerShipments) : Results.NotFound();
        })
        .WithName("GetCustomerShipments")
        .WithOpenApi();
    }
}