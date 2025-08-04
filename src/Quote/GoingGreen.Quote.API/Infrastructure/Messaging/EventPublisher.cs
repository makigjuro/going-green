using Azure.Messaging.ServiceBus;

namespace Quote.API.Infrastructure.Messaging;

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
