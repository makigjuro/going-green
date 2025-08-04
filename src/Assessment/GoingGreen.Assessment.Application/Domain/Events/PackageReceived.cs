namespace GoingGreen.Assessment.Application.Domain.Events;

public class PackageReceived
{
    public Guid AssessmentId { get; init; }
    public Guid ShipmentId { get; init; }
    public Guid QuoteId { get; init; }
    public Guid CustomerId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
    public string DeviceType { get; init; } = string.Empty;
    public string DeviceBrand { get; init; } = string.Empty;
    public string DeviceModel { get; init; } = string.Empty;
    public decimal OriginalQuoteValue { get; init; }
    public string ExpectedCondition { get; init; } = string.Empty;
    public DateTime ReceivedAt { get; init; }
    public string ReceivedBy { get; init; } = string.Empty;
    public string ReceivingLocation { get; init; } = string.Empty;

    public PackageReceived() { }

    public PackageReceived(
        Guid assessmentId,
        Guid shipmentId,
        Guid quoteId,
        Guid customerId,
        string trackingNumber,
        string deviceType,
        string deviceBrand,
        string deviceModel,
        decimal originalQuoteValue,
        string expectedCondition,
        DateTime receivedAt,
        string receivedBy,
        string receivingLocation)
    {
        AssessmentId = assessmentId;
        ShipmentId = shipmentId;
        QuoteId = quoteId;
        CustomerId = customerId;
        TrackingNumber = trackingNumber;
        DeviceType = deviceType;
        DeviceBrand = deviceBrand;
        DeviceModel = deviceModel;
        OriginalQuoteValue = originalQuoteValue;
        ExpectedCondition = expectedCondition;
        ReceivedAt = receivedAt;
        ReceivedBy = receivedBy;
        ReceivingLocation = receivingLocation;
    }
}