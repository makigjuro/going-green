using Azure.Messaging.ServiceBus;
using JasperFx;
using Marten;
using Marten.Events;
using Weasel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GoingGreen.Shipping.Application.Projections;
using GoingGreen.Shipping.Application.Services;
using JasperFx.Events.Projections;
using Shipping.API.IntegrationEvents;

namespace Shipping.API;

public static class EventingExtensions
{
    public static IHostApplicationBuilder AddEventing(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var pg = configuration.GetConnectionString("Postgres") ?? configuration["POSTGRES_CONNECTION_STRING"];
        builder.Services.AddMarten(opts =>
            {
                opts.Connection(pg);
                opts.AutoCreateSchemaObjects = AutoCreate.All;
                
                // Register projections
                opts.Projections.Add<ShipmentProjectionBuilder>(ProjectionLifecycle.Inline);
                opts.Projections.Add<CustomerShipmentsProjectionBuilder>(ProjectionLifecycle.Inline);
            })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();
        
        var sb = configuration.GetConnectionString("ServiceBus") ?? configuration["SERVICEBUS_CONNECTION_STRING"];
        if (!string.IsNullOrWhiteSpace(sb))
        {
            builder.Services.AddSingleton(new ServiceBusClient(sb));
            builder.Services.AddSingleton<IEventPublisher, ServiceBusEventPublisher>();
        }

        // Register application services
        builder.Services.AddScoped<IShippingService, ShippingService>();
        
        // Register mock implementations for external services
        builder.Services.AddScoped<IShippingLabelGenerator, Shipping.API.Services.MockShippingLabelGenerator>();
        builder.Services.AddScoped<IShippingCarrierNotifier, Shipping.API.Services.MockShippingCarrierNotifier>();

        // Register integration event handlers
        if (!string.IsNullOrWhiteSpace(sb))
        {
            builder.Services.AddHostedService<QuoteAcceptedIntegrationEventHandler>();
        }

        return builder;
    }
}

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default);
}

public class ServiceBusEventPublisher : IEventPublisher
{
    private readonly ServiceBusClient _client;

    public ServiceBusEventPublisher(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
    {
        var sender = _client.CreateSender(typeof(T).Name);
        var body = System.Text.Json.JsonSerializer.Serialize(@event);
        await sender.SendMessageAsync(new ServiceBusMessage(body), cancellationToken);
    }
}
