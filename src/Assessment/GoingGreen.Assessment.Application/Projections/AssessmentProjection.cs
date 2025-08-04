using GoingGreen.Assessment.Application.Domain.Events;
using GoingGreen.Assessment.Application.Domain.ValueObjects;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace GoingGreen.Assessment.Application.Projections;

public class AssessmentProjection
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Guid QuoteId { get; set; }
    public Guid CustomerId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceBrand { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public decimal OriginalQuoteValue { get; set; }
    public string ExpectedCondition { get; set; } = string.Empty;
    public AssessmentStatus Status { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string ReceivedBy { get; set; } = string.Empty;
    public string ReceivingLocation { get; set; } = string.Empty;
    public string? InspectorId { get; set; }
    public string? InspectorName { get; set; }
    public Dictionary<string, object> InspectionResults { get; set; } = new();
    public string? ActualCondition { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Photos { get; set; } = new();
    public string? InspectionNotes { get; set; }
    public string? Classification { get; set; }
    public string? ResaleGrade { get; set; }
    public decimal? FinalOffer { get; set; }
    public string? ClassificationReason { get; set; }
    public DateTime? InspectionStartedAt { get; set; }
    public DateTime? InspectionCompletedAt { get; set; }
    public DateTime? ClassifiedAt { get; set; }
    public DateTime? ReportGeneratedAt { get; set; }
    public string? ReportId { get; set; }
    public string? ReportUrl { get; set; }
    public decimal? AdjustmentAmount { get; set; }
    public string? AdjustmentReason { get; set; }
    public bool RequiresCustomerApproval { get; set; }
}

public class AssessmentProjectionBuilder : SingleStreamProjection<AssessmentProjection, Guid>
{
    public AssessmentProjection Create(PackageReceived @event)
    {
        return new AssessmentProjection
        {
            Id = @event.AssessmentId,
            ShipmentId = @event.ShipmentId,
            QuoteId = @event.QuoteId,
            CustomerId = @event.CustomerId,
            TrackingNumber = @event.TrackingNumber,
            DeviceType = @event.DeviceType,
            DeviceBrand = @event.DeviceBrand,
            DeviceModel = @event.DeviceModel,
            OriginalQuoteValue = @event.OriginalQuoteValue,
            ExpectedCondition = @event.ExpectedCondition,
            Status = AssessmentStatus.PackageReceived,
            ReceivedAt = @event.ReceivedAt,
            ReceivedBy = @event.ReceivedBy,
            ReceivingLocation = @event.ReceivingLocation
        };
    }

    public void Apply(InspectionStarted @event, AssessmentProjection projection)
    {
        projection.Status = AssessmentStatus.InspectionStarted;
        projection.InspectorId = @event.InspectorId;
        projection.InspectorName = @event.InspectorName;
        projection.InspectionStartedAt = @event.StartedAt;
    }

    public void Apply(InspectionCompleted @event, AssessmentProjection projection)
    {
        projection.Status = AssessmentStatus.InspectionCompleted;
        projection.InspectionResults = @event.InspectionResults;
        projection.ActualCondition = @event.ActualCondition;
        projection.Issues = @event.Issues;
        projection.Photos = @event.Photos;
        projection.InspectionNotes = @event.Notes;
        projection.InspectionCompletedAt = @event.CompletedAt;
    }

    public void Apply(DeviceClassified @event, AssessmentProjection projection)
    {
        projection.Status = AssessmentStatus.DeviceClassified;
        projection.Classification = @event.Classification;
        projection.ResaleGrade = @event.ResaleGrade;
        projection.FinalOffer = @event.ResaleValue ?? @event.RecyclingValue;
        projection.ClassificationReason = @event.ClassificationReason;
        projection.ClassifiedAt = @event.ClassifiedAt;
    }

    public void Apply(OfferRecalculated @event, AssessmentProjection projection)
    {
        projection.Status = AssessmentStatus.OfferRecalculated;
        projection.FinalOffer = @event.NewOffer;
        projection.AdjustmentAmount = @event.AdjustmentAmount;
        projection.AdjustmentReason = @event.AdjustmentReason;
        projection.RequiresCustomerApproval = @event.RequiresCustomerApproval;
    }

    public void Apply(AssessmentReportGenerated @event, AssessmentProjection projection)
    {
        projection.Status = AssessmentStatus.ReportGenerated;
        projection.ReportId = @event.ReportId;
        projection.ReportUrl = @event.ReportUrl;
        projection.FinalOffer = @event.FinalOffer;
        projection.Classification = @event.FinalClassification;
        projection.ReportGeneratedAt = @event.GeneratedAt;
    }
}