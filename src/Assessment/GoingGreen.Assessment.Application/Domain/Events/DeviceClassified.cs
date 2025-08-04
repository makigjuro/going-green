namespace GoingGreen.Assessment.Application.Domain.Events;

public class DeviceClassified
{
    public Guid AssessmentId { get; init; }
    public string Classification { get; init; } = string.Empty; // "Resellable" or "Recycle"
    public string ClassificationReason { get; init; } = string.Empty;
    public decimal? ResaleValue { get; init; }
    public string ResaleGrade { get; init; } = string.Empty; // A, B, C, D
    public string RecyclingCategory { get; init; } = string.Empty;
    public decimal RecyclingValue { get; init; }
    public DateTime ClassifiedAt { get; init; }

    public DeviceClassified() { }

    public DeviceClassified(
        Guid assessmentId,
        string classification,
        string classificationReason,
        decimal? resaleValue,
        string resaleGrade,
        string recyclingCategory,
        decimal recyclingValue,
        DateTime classifiedAt)
    {
        AssessmentId = assessmentId;
        Classification = classification;
        ClassificationReason = classificationReason;
        ResaleValue = resaleValue;
        ResaleGrade = resaleGrade;
        RecyclingCategory = recyclingCategory;
        RecyclingValue = recyclingValue;
        ClassifiedAt = classifiedAt;
    }
}