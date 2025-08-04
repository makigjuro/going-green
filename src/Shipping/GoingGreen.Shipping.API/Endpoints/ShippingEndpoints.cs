using GoingGreen.Shipping.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shipping.API.Models;

namespace Shipping.API.Endpoints;

public static class ShippingEndpoints
{
    public static void MapShippingEndpoints(this IEndpointRouteBuilder app)
    {
        var shipping = app.MapGroup("/shipments").WithTags("Shipping");

        shipping.MapPost("/", async (
            [FromBody] CreateShipmentRequest request,
            [FromServices] IShippingService shippingService) =>
        {
            var shipmentId = await shippingService.RequestShippingAsync(
                request.QuoteId,
                request.CustomerId,
                request.CustomerName,
                request.CustomerEmail,
                request.DeviceType,
                request.DeviceBrand,
                request.DeviceModel,
                request.QuoteValue);

            return Results.Created($"/shipments/{shipmentId}", new { ShipmentId = shipmentId });
        })
        .WithName("CreateShipment")
        .WithOpenApi();

        shipping.MapGet("/{shipmentId:guid}", async (
            Guid shipmentId,
            [FromServices] IShippingService shippingService) =>
        {
            var shipment = await shippingService.GetShipmentAsync(shipmentId);
            return shipment is not null ? Results.Ok(shipment) : Results.NotFound();
        })
        .WithName("GetShipment")
        .WithOpenApi();

        shipping.MapGet("/tracking/{trackingNumber}", async (
            string trackingNumber,
            [FromServices] IShippingService shippingService) =>
        {
            var shipment = await shippingService.GetShipmentByTrackingNumberAsync(trackingNumber);
            return shipment is not null ? Results.Ok(shipment) : Results.NotFound();
        })
        .WithName("GetShipmentByTracking")
        .WithOpenApi();

        shipping.MapPost("/{shipmentId:guid}/generate-label", async (
            Guid shipmentId,
            [FromServices] IShippingService shippingService) =>
        {
            try
            {
                await shippingService.GenerateShippingLabelAsync(shipmentId);
                return Results.Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("GenerateShippingLabel")
        .WithOpenApi();

        shipping.MapPost("/{shipmentId:guid}/tracking", async (
            Guid shipmentId,
            [FromBody] UpdateTrackingRequest request,
            [FromServices] IShippingService shippingService) =>
        {
            try
            {
                await shippingService.UpdateTrackingInfoAsync(
                    shipmentId,
                    request.Status,
                    request.StatusDescription,
                    request.Location,
                    request.EstimatedDeliveryDate);
                return Results.Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("UpdateTrackingInfo")
        .WithOpenApi();

        shipping.MapPost("/{shipmentId:guid}/deliver", async (
            Guid shipmentId,
            [FromBody] DeliveryRequest request,
            [FromServices] IShippingService shippingService) =>
        {
            try
            {
                await shippingService.MarkAsDeliveredAsync(
                    shipmentId,
                    request.DeliveredTo,
                    request.DeliveryLocation);
                return Results.Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("MarkAsDelivered")
        .WithOpenApi();

        shipping.MapPost("/{shipmentId:guid}/cancel", async (
            Guid shipmentId,
            [FromBody] CancelShipmentRequest request,
            [FromServices] IShippingService shippingService) =>
        {
            try
            {
                await shippingService.CancelShippingAsync(shipmentId, request.Reason);
                return Results.Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("CancelShipment")
        .WithOpenApi();
    }
}