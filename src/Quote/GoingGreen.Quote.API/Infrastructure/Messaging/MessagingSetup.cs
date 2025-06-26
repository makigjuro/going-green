using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Quote.API.Infrastructure.Messaging;

public static class MessagingSetup
{
    public static IHostApplicationBuilder AddMessaging(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var sb = configuration.GetConnectionString("ServiceBus") ?? configuration["SERVICEBUS_CONNECTION_STRING"];
        if (!string.IsNullOrWhiteSpace(sb))
        {
            builder.Services.AddSingleton(new ServiceBusClient(sb));
            builder.Services.AddSingleton<IEventPublisher, ServiceBusEventPublisher>();
            builder.Services.AddHostedService<AzureServiceBusListener>();
        }
        return builder;
    }
}
