namespace GoingGreen.Assessment.Application.Domain.ValueObjects;

public enum AssessmentStatus
{
    PackageReceived,
    InspectionStarted,
    InspectionCompleted,
    DeviceClassified,
    OfferRecalculated,
    ReportGenerated,
    Completed,
    Rejected
}