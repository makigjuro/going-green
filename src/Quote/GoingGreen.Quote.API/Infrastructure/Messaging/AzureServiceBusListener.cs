using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Quote.API.Infrastructure.Messaging;

public class AzureServiceBusListener : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<AzureServiceBusListener> _logger;

    public AzureServiceBusListener(ServiceBusClient client, ILogger<AzureServiceBusListener> logger)
    {
        _logger = logger;
        _processor = client.CreateProcessor("quotes", new ServiceBusProcessorOptions());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync(stoppingToken);
    }

    private Task MessageHandler(ProcessMessageEventArgs args)
    {
        _logger.LogInformation("Received message {Body}", args.Message.Body.ToString());
        return Task.CompletedTask;
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error processing message");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
