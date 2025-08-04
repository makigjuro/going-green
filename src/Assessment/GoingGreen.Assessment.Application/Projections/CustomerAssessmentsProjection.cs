using GoingGreen.Assessment.Application.Domain.Events;
using GoingGreen.Assessment.Application.Domain.ValueObjects;
using Marten.Events.Projections;

namespace GoingGreen.Assessment.Application.Projections;

public class CustomerAssessmentsProjection
{
    public Guid CustomerId { get; set; }
    public List<CustomerAssessmentItem> Assessments { get; set; } = new();
    public int TotalAssessments { get; set; }
    public int CompletedAssessments { get; set; }
    public int ResellableDevices { get; set; }
    public int RecycledDevices { get; set; }
    public decimal TotalOriginalValue { get; set; }
    public decimal TotalFinalValue { get; set; }
    public decimal TotalAdjustment { get; set; }
}

public class CustomerAssessmentItem
{
    public Guid AssessmentId { get; set; }
    public Guid QuoteId { get; set; }
    public Guid ShipmentId { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceBrand { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public AssessmentStatus Status { get; set; }
    public decimal OriginalQuoteValue { get; set; }
    public decimal? FinalOffer { get; set; }
    public string? Classification { get; set; }
    public string? ResaleGrade { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CustomerAssessmentsProjectionBuilder : MultiStreamProjection<CustomerAssessmentsProjection, Guid>
{
    public CustomerAssessmentsProjectionBuilder()
    {
        Identity<PackageReceived>(x => x.CustomerId);
        Identity<DeviceClassified>(x => x.AssessmentId); // This will need custom handling
        Identity<AssessmentReportGenerated>(x => x.CustomerId);
    }

    public CustomerAssessmentsProjection Create(PackageReceived @event)
    {
        return new CustomerAssessmentsProjection
        {
            CustomerId = @event.CustomerId,
            Assessments = new List<CustomerAssessmentItem>
            {
                new()
                {
                    AssessmentId = @event.AssessmentId,
                    QuoteId = @event.QuoteId,
                    ShipmentId = @event.ShipmentId,
                    DeviceType = @event.DeviceType,
                    DeviceBrand = @event.DeviceBrand,
                    DeviceModel = @event.DeviceModel,
                    Status = AssessmentStatus.PackageReceived,
                    OriginalQuoteValue = @event.OriginalQuoteValue,
                    ReceivedAt = @event.ReceivedAt
                }
            },
            TotalAssessments = 1,
            TotalOriginalValue = @event.OriginalQuoteValue
        };
    }

    public void Apply(PackageReceived @event, CustomerAssessmentsProjection projection)
    {
        var existingAssessment = projection.Assessments.FirstOrDefault(a => a.AssessmentId == @event.AssessmentId);
        if (existingAssessment == null)
        {
            projection.Assessments.Add(new CustomerAssessmentItem
            {
                AssessmentId = @event.AssessmentId,
                QuoteId = @event.QuoteId,
                ShipmentId = @event.ShipmentId,
                DeviceType = @event.DeviceType,
                DeviceBrand = @event.DeviceBrand,
                DeviceModel = @event.DeviceModel,
                Status = AssessmentStatus.PackageReceived,
                OriginalQuoteValue = @event.OriginalQuoteValue,
                ReceivedAt = @event.ReceivedAt
            });
            projection.TotalAssessments++;
            projection.TotalOriginalValue += @event.OriginalQuoteValue;
        }
    }

    public void Apply(DeviceClassified @event, CustomerAssessmentsProjection projection)
    {
        var assessment = projection.Assessments.FirstOrDefault(a => a.AssessmentId == @event.AssessmentId);
        if (assessment != null)
        {
            assessment.Status = AssessmentStatus.DeviceClassified;
            assessment.Classification = @event.Classification;
            assessment.ResaleGrade = @event.ResaleGrade;
            assessment.FinalOffer = @event.ResaleValue ?? @event.RecyclingValue;

            if (@event.Classification.Equals("Resellable", StringComparison.OrdinalIgnoreCase))
            {
                projection.ResellableDevices++;
            }
            else if (@event.Classification.Equals("Recycle", StringComparison.OrdinalIgnoreCase))
            {
                projection.RecycledDevices++;
            }
        }
    }

    public void Apply(AssessmentReportGenerated @event, CustomerAssessmentsProjection projection)
    {
        var assessment = projection.Assessments.FirstOrDefault(a => a.AssessmentId == @event.AssessmentId);
        if (assessment != null)
        {
            assessment.Status = AssessmentStatus.ReportGenerated;
            assessment.FinalOffer = @event.FinalOffer;
            assessment.Classification = @event.FinalClassification;
            assessment.CompletedAt = @event.GeneratedAt;

            projection.CompletedAssessments++;
            projection.TotalFinalValue += @event.FinalOffer;
            projection.TotalAdjustment += (@event.FinalOffer - assessment.OriginalQuoteValue);
        }
    }
}