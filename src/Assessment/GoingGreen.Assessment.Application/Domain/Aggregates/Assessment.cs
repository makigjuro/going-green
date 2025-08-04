using GoingGreen.Assessment.Application.Domain.Events;
using GoingGreen.Assessment.Application.Domain.ValueObjects;
using GoingGreen.Assessment.Application.Domain.Services;

namespace GoingGreen.Assessment.Application.Domain.Aggregates;

public class Assessment
{
    public Guid Id { get; private set; }
    public Guid ShipmentId { get; private set; }
    public Guid QuoteId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string TrackingNumber { get; private set; } = string.Empty;
    public string DeviceType { get; private set; } = string.Empty;
    public string DeviceBrand { get; private set; } = string.Empty;
    public string DeviceModel { get; private set; } = string.Empty;
    public decimal OriginalQuoteValue { get; private set; }
    public string ExpectedCondition { get; private set; } = string.Empty;
    public AssessmentStatus Status { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public string ReceivedBy { get; private set; } = string.Empty;
    public string ReceivingLocation { get; private set; } = string.Empty;
    public string? InspectorId { get; private set; }
    public string? InspectorName { get; private set; }
    public Dictionary<string, object> InspectionResults { get; private set; } = new();
    public string? ActualCondition { get; private set; }
    public List<string> Issues { get; private set; } = new();
    public List<string> Photos { get; private set; } = new();
    public string? InspectionNotes { get; private set; }
    public DeviceClassification? Classification { get; private set; }
    public ResaleGrade? ResaleGrade { get; private set; }
    public decimal? FinalOffer { get; private set; }
    public string? ClassificationReason { get; private set; }
    public DateTime? InspectionStartedAt { get; private set; }
    public DateTime? InspectionCompletedAt { get; private set; }
    public DateTime? ClassifiedAt { get; private set; }
    public DateTime? ReportGeneratedAt { get; private set; }
    public string? ReportId { get; private set; }
    public string? ReportUrl { get; private set; }

    private Assessment() { }

    public static Assessment ReceivePackage(
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
        var assessment = new Assessment
        {
            Id = assessmentId,
            ShipmentId = shipmentId,
            QuoteId = quoteId,
            CustomerId = customerId,
            TrackingNumber = trackingNumber,
            DeviceType = deviceType,
            DeviceBrand = deviceBrand,
            DeviceModel = deviceModel,
            OriginalQuoteValue = originalQuoteValue,
            ExpectedCondition = expectedCondition,
            Status = AssessmentStatus.PackageReceived,
            ReceivedAt = receivedAt,
            ReceivedBy = receivedBy,
            ReceivingLocation = receivingLocation
        };

        return assessment;
    }

    public void StartInspection(
        string inspectorId,
        string inspectorName,
        List<string> inspectionCriteria,
        DateTime startedAt)
    {
        if (Status != AssessmentStatus.PackageReceived)
            throw new InvalidOperationException($"Cannot start inspection for assessment in status {Status}");

        InspectorId = inspectorId;
        InspectorName = inspectorName;
        InspectionStartedAt = startedAt;
        Status = AssessmentStatus.InspectionStarted;
    }

    public void CompleteInspection(
        Dictionary<string, object> inspectionResults,
        string actualCondition,
        List<string> issues,
        List<string> photos,
        string notes,
        DateTime completedAt)
    {
        if (Status != AssessmentStatus.InspectionStarted)
            throw new InvalidOperationException($"Cannot complete inspection for assessment in status {Status}");

        InspectionResults = inspectionResults;
        ActualCondition = actualCondition;
        Issues = issues;
        Photos = photos;
        InspectionNotes = notes;
        InspectionCompletedAt = completedAt;
        Status = AssessmentStatus.InspectionCompleted;
    }

    public void ClassifyDevice(
        IInspectionRulesEngine rulesEngine,
        DateTime classifiedAt)
    {
        if (Status != AssessmentStatus.InspectionCompleted)
            throw new InvalidOperationException($"Cannot classify device for assessment in status {Status}");

        var classificationResult = rulesEngine.ClassifyDevice(DeviceType, InspectionResults, OriginalQuoteValue);
        
        Classification = classificationResult.Classification;
        ResaleGrade = classificationResult.ResaleGrade;
        FinalOffer = classificationResult.Value;
        ClassificationReason = classificationResult.Reason;
        ClassifiedAt = classifiedAt;
        Status = AssessmentStatus.DeviceClassified;
    }

    public void RecalculateOffer(
        decimal newOffer,
        string adjustmentReason,
        List<string> conditionMismatches,
        DateTime recalculatedAt)
    {
        if (Status != AssessmentStatus.DeviceClassified)
            throw new InvalidOperationException($"Cannot recalculate offer for assessment in status {Status}");

        FinalOffer = newOffer;
        Status = AssessmentStatus.OfferRecalculated;
    }

    public void GenerateReport(
        string reportId,
        string reportUrl,
        Dictionary<string, object> reportData,
        DateTime generatedAt)
    {
        if (Status != AssessmentStatus.DeviceClassified && Status != AssessmentStatus.OfferRecalculated)
            throw new InvalidOperationException($"Cannot generate report for assessment in status {Status}");

        ReportId = reportId;
        ReportUrl = reportUrl;
        ReportGeneratedAt = generatedAt;
        Status = AssessmentStatus.ReportGenerated;
    }

    public void Complete()
    {
        if (Status != AssessmentStatus.ReportGenerated)
            throw new InvalidOperationException($"Cannot complete assessment in status {Status}");

        Status = AssessmentStatus.Completed;
    }

    public bool RequiresCustomerApproval()
    {
        if (FinalOffer == null)
            return false;

        var adjustmentPercentage = Math.Abs((FinalOffer.Value - OriginalQuoteValue) / OriginalQuoteValue);
        return adjustmentPercentage > 0.1m; // Requires approval if adjustment > 10%
    }
}