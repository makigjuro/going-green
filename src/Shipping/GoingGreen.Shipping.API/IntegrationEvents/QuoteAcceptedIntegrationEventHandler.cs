using Azure.Messaging.ServiceBus;
using GoingGreen.Shipping.Application.Services;
using System.Text.Json;

namespace Shipping.API.IntegrationEvents;

public class QuoteAcceptedIntegrationEventHandler : IHostedService
{
    private readonly ServiceBusProcessor _processor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QuoteAcceptedIntegrationEventHandler> _logger;

    public QuoteAcceptedIntegrationEventHandler(
        ServiceBusClient serviceBusClient,
        IServiceProvider serviceProvider,
        ILogger<QuoteAcceptedIntegrationEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _processor = serviceBusClient.CreateProcessor("QuoteAccepted", "shipping-service", new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += HandleMessageAsync;
        _processor.ProcessErrorAsync += HandleErrorAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _processor.StartProcessingAsync(cancellationToken);
        _logger.LogInformation("QuoteAccepted integration event handler started");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        _logger.LogInformation("QuoteAccepted integration event handler stopped");
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var messageBody = args.Message.Body.ToString();
            var integrationEvent = JsonSerializer.Deserialize<QuoteAcceptedIntegrationEvent>(messageBody);

            if (integrationEvent == null)
            {
                _logger.LogWarning("Failed to deserialize QuoteAccepted integration event");
                return;
            }

            _logger.LogInformation("Processing QuoteAccepted integration event for Quote {QuoteId}", integrationEvent.QuoteId);

            using var scope = _serviceProvider.CreateScope();
            var shippingService = scope.ServiceProvider.GetRequiredService<IShippingService>();

            // Automatically create shipment when quote is accepted
            var shipmentId = await shippingService.RequestShippingAsync(
                integrationEvent.QuoteId,
                integrationEvent.CustomerId,
                integrationEvent.CustomerName,
                integrationEvent.CustomerEmail,
                integrationEvent.DeviceType,
                integrationEvent.DeviceBrand,
                integrationEvent.DeviceModel,
                integrationEvent.EstimatedValue);

            _logger.LogInformation("Created shipment {ShipmentId} for accepted quote {QuoteId}", 
                shipmentId, integrationEvent.QuoteId);

            // Automatically generate shipping label
            await shippingService.GenerateShippingLabelAsync(shipmentId);

            _logger.LogInformation("Generated shipping label for shipment {ShipmentId}", shipmentId);

            // Complete the message
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing QuoteAccepted integration event");
            // Don't complete the message - it will be retried
            throw;
        }
    }

    private Task HandleErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error in QuoteAccepted message processing: {ErrorSource}", args.ErrorSource);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _processor?.DisposeAsync().AsTask().Wait();
    }
}