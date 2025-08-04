using GoingGreen.Assessment.Application.Services;
using System.Text.Json;

namespace GoingGreen.Assessment.API.IntegrationEventHandlers;

public class PackageDeliveredEventHandler
{
    private readonly IAssessmentService _assessmentService;
    private readonly ILogger<PackageDeliveredEventHandler> _logger;

    public PackageDeliveredEventHandler(
        IAssessmentService assessmentService,
        ILogger<PackageDeliveredEventHandler> logger)
    {
        _assessmentService = assessmentService;
        _logger = logger;
    }

    public async Task HandleAsync(PackageDeliveredIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing PackageDelivered integration event for shipment {ShipmentId}", integrationEvent.ShipmentId);

        try
        {
            var assessmentId = await _assessmentService.ReceivePackageAsync(
                integrationEvent.ShipmentId,
                integrationEvent.QuoteId,
                integrationEvent.CustomerId,
                integrationEvent.TrackingNumber,
                integrationEvent.DeviceType,
                integrationEvent.DeviceBrand,
                integrationEvent.DeviceModel,
                integrationEvent.OriginalQuoteValue,
                integrationEvent.ExpectedCondition,
                "Assessment Facility",
                integrationEvent.DeliveryLocation,
                cancellationToken);

            _logger.LogInformation("Created assessment {AssessmentId} for delivered package {TrackingNumber}", 
                assessmentId, integrationEvent.TrackingNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process PackageDelivered integration event for shipment {ShipmentId}", 
                integrationEvent.ShipmentId);
            throw;
        }
    }
}

public record PackageDeliveredIntegrationEvent(
    Guid EventId,
    DateTime OccurredOn,
    Guid ShipmentId,
    Guid QuoteId,
    Guid CustomerId,
    string TrackingNumber,
    string DeviceType,
    string DeviceBrand,
    string DeviceModel,
    decimal OriginalQuoteValue,
    string ExpectedCondition,
    string DeliveryLocation,
    DateTime DeliveredAt);