namespace GoingGreen.Assessment.Application.Domain.Events;

public class AssessmentReportGenerated
{
    public Guid AssessmentId { get; init; }
    public Guid QuoteId { get; init; }
    public Guid CustomerId { get; init; }
    public string ReportId { get; init; } = string.Empty;
    public string ReportUrl { get; init; } = string.Empty;
    public Dictionary<string, object> ReportData { get; init; } = new();
    public string FinalClassification { get; init; } = string.Empty;
    public decimal FinalOffer { get; init; }
    public string ReportFormat { get; init; } = string.Empty; // PDF, JSON, etc.
    public DateTime GeneratedAt { get; init; }

    public AssessmentReportGenerated() { }

    public AssessmentReportGenerated(
        Guid assessmentId,
        Guid quoteId,
        Guid customerId,
        string reportId,
        string reportUrl,
        Dictionary<string, object> reportData,
        string finalClassification,
        decimal finalOffer,
        string reportFormat,
        DateTime generatedAt)
    {
        AssessmentId = assessmentId;
        QuoteId = quoteId;
        CustomerId = customerId;
        ReportId = reportId;
        ReportUrl = reportUrl;
        ReportData = reportData;
        FinalClassification = finalClassification;
        FinalOffer = finalOffer;
        ReportFormat = reportFormat;
        GeneratedAt = generatedAt;
    }
}