using Azure.Messaging.ServiceBus;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Payment.API;

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
            opts.UseDefaultSerialization(nonPublicMembersStorage: NonPublicMembersStorage.All);
        }).UseLightweightSessions().UseEventing();

        var sb = configuration.GetConnectionString("ServiceBus") ?? configuration["SERVICEBUS_CONNECTION_STRING"];
        if (!string.IsNullOrWhiteSpace(sb))
        {
            builder.Services.AddSingleton(new ServiceBusClient(sb));
            builder.Services.AddSingleton<IEventPublisher, ServiceBusEventPublisher>();
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
