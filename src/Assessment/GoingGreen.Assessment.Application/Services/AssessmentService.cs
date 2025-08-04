using GoingGreen.Assessment.Application.Domain.Aggregates;
using GoingGreen.Assessment.Application.Domain.Events;
using GoingGreen.Assessment.Application.Domain.Services;
using GoingGreen.Assessment.Application.Domain.ValueObjects;
using GoingGreen.Assessment.Application.Projections;
using Marten;

namespace GoingGreen.Assessment.Application.Services;

public interface IAssessmentService
{
    Task<Guid> ReceivePackageAsync(Guid shipmentId, Guid quoteId, Guid customerId, string trackingNumber,
        string deviceType, string deviceBrand, string deviceModel, decimal originalQuoteValue, 
        string expectedCondition, string receivedBy, string receivingLocation, 
        CancellationToken cancellationToken = default);
    
    Task<AssessmentProjection?> GetAssessmentAsync(Guid assessmentId, CancellationToken cancellationToken = default);
    Task<CustomerAssessmentsProjection?> GetCustomerAssessmentsAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<List<InspectionCriterion>> GetInspectionCriteriaAsync(string deviceType, CancellationToken cancellationToken = default);
    
    Task StartInspectionAsync(Guid assessmentId, string inspectorId, string inspectorName, 
        CancellationToken cancellationToken = default);
    
    Task CompleteInspectionAsync(Guid assessmentId, Dictionary<string, object> inspectionResults,
        string actualCondition, List<string> issues, List<string> photos, string notes,
        CancellationToken cancellationToken = default);
    
    Task GenerateReportAsync(Guid assessmentId, CancellationToken cancellationToken = default);
}

public class AssessmentService : IAssessmentService
{
    private readonly IDocumentSession _session;
    private readonly IInspectionRulesEngine _rulesEngine;
    private readonly IAssessmentReportGenerator _reportGenerator;

    public AssessmentService(
        IDocumentSession session,
        IInspectionRulesEngine rulesEngine,
        IAssessmentReportGenerator reportGenerator)
    {
        _session = session;
        _rulesEngine = rulesEngine;
        _reportGenerator = reportGenerator;
    }

    public async Task<Guid> ReceivePackageAsync(Guid shipmentId, Guid quoteId, Guid customerId, string trackingNumber,
        string deviceType, string deviceBrand, string deviceModel, decimal originalQuoteValue, 
        string expectedCondition, string receivedBy, string receivingLocation, 
        CancellationToken cancellationToken = default)
    {
        var assessmentId = Guid.NewGuid();
        var receivedAt = DateTime.UtcNow;

        var packageReceived = new PackageReceived(
            assessmentId, shipmentId, quoteId, customerId, trackingNumber,
            deviceType, deviceBrand, deviceModel, originalQuoteValue, expectedCondition,
            receivedAt, receivedBy, receivingLocation);

        _session.Events.StartStream<Domain.Aggregates.Assessment>(assessmentId, packageReceived);
        await _session.SaveChangesAsync(cancellationToken);

        return assessmentId;
    }

    public async Task<AssessmentProjection?> GetAssessmentAsync(Guid assessmentId, CancellationToken cancellationToken = default)
    {
        return await _session.Query<AssessmentProjection>()
            .FirstOrDefaultAsync(a => a.Id == assessmentId, cancellationToken);
    }

    public async Task<CustomerAssessmentsProjection?> GetCustomerAssessmentsAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _session.Query<CustomerAssessmentsProjection>()
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
    }

    public Task<List<InspectionCriterion>> GetInspectionCriteriaAsync(string deviceType, CancellationToken cancellationToken = default)
    {
        var criteria = _rulesEngine.GetInspectionCriteria(deviceType);
        return Task.FromResult(criteria);
    }

    public async Task StartInspectionAsync(Guid assessmentId, string inspectorId, string inspectorName, 
        CancellationToken cancellationToken = default)
    {
        var assessment = await GetAssessmentAsync(assessmentId, cancellationToken);
        if (assessment == null)
            throw new InvalidOperationException($"Assessment {assessmentId} not found");

        var inspectionCriteria = _rulesEngine.GetInspectionCriteria(assessment.DeviceType)
            .Select(c => c.Name).ToList();

        var inspectionStarted = new InspectionStarted(
            assessmentId,
            inspectorId,
            inspectorName,
            inspectionCriteria,
            DateTime.UtcNow);

        _session.Events.Append(assessmentId, inspectionStarted);
        await _session.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteInspectionAsync(Guid assessmentId, Dictionary<string, object> inspectionResults,
        string actualCondition, List<string> issues, List<string> photos, string notes,
        CancellationToken cancellationToken = default)
    {
        var assessment = await GetAssessmentAsync(assessmentId, cancellationToken);
        if (assessment == null)
            throw new InvalidOperationException($"Assessment {assessmentId} not found");

        var completedAt = DateTime.UtcNow;

        // Complete inspection
        var inspectionCompleted = new InspectionCompleted(
            assessmentId,
            assessment.InspectorId ?? "",
            inspectionResults,
            actualCondition,
            issues,
            photos,
            notes,
            completedAt);

        _session.Events.Append(assessmentId, inspectionCompleted);

        // Classify device based on inspection results
        var classificationResult = _rulesEngine.ClassifyDevice(
            assessment.DeviceType, 
            inspectionResults, 
            assessment.OriginalQuoteValue);

        var deviceClassified = new DeviceClassified(
            assessmentId,
            classificationResult.Classification.ToString(),
            classificationResult.Reason,
            classificationResult.Classification == DeviceClassification.Resellable ? classificationResult.Value : null,
            classificationResult.ResaleGrade?.ToString() ?? "",
            classificationResult.Classification == DeviceClassification.Recycle ? "Electronic Components" : "",
            classificationResult.Classification == DeviceClassification.Recycle ? classificationResult.Value : 0,
            completedAt);

        _session.Events.Append(assessmentId, deviceClassified);

        // Check if offer needs recalculation
        var adjustmentAmount = classificationResult.Value - assessment.OriginalQuoteValue;
        var requiresApproval = Math.Abs(adjustmentAmount) > (assessment.OriginalQuoteValue * 0.1m);

        if (adjustmentAmount != 0)
        {
            var offerRecalculated = new OfferRecalculated(
                assessmentId,
                assessment.QuoteId,
                assessment.OriginalQuoteValue,
                classificationResult.Value,
                adjustmentAmount,
                classificationResult.Reason,
                issues,
                completedAt,
                requiresApproval);

            _session.Events.Append(assessmentId, offerRecalculated);
        }

        await _session.SaveChangesAsync(cancellationToken);
    }

    public async Task GenerateReportAsync(Guid assessmentId, CancellationToken cancellationToken = default)
    {
        var assessment = await GetAssessmentAsync(assessmentId, cancellationToken);
        if (assessment == null)
            throw new InvalidOperationException($"Assessment {assessmentId} not found");

        var reportData = new Dictionary<string, object>
        {
            ["assessmentId"] = assessment.Id,
            ["deviceType"] = assessment.DeviceType,
            ["deviceBrand"] = assessment.DeviceBrand,
            ["deviceModel"] = assessment.DeviceModel,
            ["originalQuoteValue"] = assessment.OriginalQuoteValue,
            ["finalOffer"] = assessment.FinalOffer ?? 0,
            ["classification"] = assessment.Classification ?? "",
            ["resaleGrade"] = assessment.ResaleGrade ?? "",
            ["inspectionResults"] = assessment.InspectionResults,
            ["issues"] = assessment.Issues,
            ["photos"] = assessment.Photos,
            ["inspectionNotes"] = assessment.InspectionNotes ?? "",
            ["classificationReason"] = assessment.ClassificationReason ?? ""
        };

        var reportResult = await _reportGenerator.GenerateReportAsync(assessment.Id, reportData);

        var reportGenerated = new AssessmentReportGenerated(
            assessmentId,
            assessment.QuoteId,
            assessment.CustomerId,
            reportResult.ReportId,
            reportResult.ReportUrl,
            reportData,
            assessment.Classification ?? "",
            assessment.FinalOffer ?? 0,
            "PDF",
            DateTime.UtcNow);

        _session.Events.Append(assessmentId, reportGenerated);
        await _session.SaveChangesAsync(cancellationToken);
    }
}

// External service interfaces
public interface IAssessmentReportGenerator
{
    Task<AssessmentReportResult> GenerateReportAsync(Guid assessmentId, Dictionary<string, object> reportData);
}

public record AssessmentReportResult(
    string ReportId,
    string ReportUrl);