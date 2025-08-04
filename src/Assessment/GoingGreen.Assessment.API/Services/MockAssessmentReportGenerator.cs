using GoingGreen.Assessment.Application.Services;

namespace GoingGreen.Assessment.API.Services;

public class MockAssessmentReportGenerator : IAssessmentReportGenerator
{
    public Task<AssessmentReportResult> GenerateReportAsync(Guid assessmentId, Dictionary<string, object> reportData)
    {
        var reportId = $"RPT-{DateTime.UtcNow:yyyyMMdd}-{assessmentId.ToString()[..8].ToUpperInvariant()}";
        var reportUrl = $"https://reports.goinggreen.com/assessments/{reportId}.pdf";

        var result = new AssessmentReportResult(reportId, reportUrl);
        
        return Task.FromResult(result);
    }
}