using GoingGreen.Assessment.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoingGreen.Assessment.API.Endpoints;

public static class AssessmentEndpoints
{
    public static void MapAssessmentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/assessments").WithTags("Assessments");

        group.MapPost("/packages/{shipmentId}/receive", ReceivePackage)
            .WithName("ReceivePackage")
            .WithSummary("Receive package for assessment")
            .WithDescription("Records package receipt at assessment facility and creates new assessment");

        group.MapGet("/{id}", GetAssessment)
            .WithName("GetAssessment")
            .WithSummary("Get assessment details")
            .WithDescription("Retrieves assessment information by ID");

        group.MapGet("/customers/{customerId}", GetCustomerAssessments)
            .WithName("GetCustomerAssessments")
            .WithSummary("Get customer assessments")
            .WithDescription("Retrieves all assessments for a specific customer");

        group.MapGet("/devices/{deviceType}/criteria", GetInspectionCriteria)
            .WithName("GetInspectionCriteria")
            .WithSummary("Get inspection criteria")
            .WithDescription("Retrieves inspection criteria for a specific device type");

        group.MapPost("/{id}/inspection/start", StartInspection)
            .WithName("StartInspection")
            .WithSummary("Start device inspection")
            .WithDescription("Starts the inspection process for an assessment");

        group.MapPost("/{id}/inspection/complete", CompleteInspection)
            .WithName("CompleteInspection")
            .WithSummary("Complete device inspection")
            .WithDescription("Completes the inspection process with results and classification");

        group.MapPost("/{id}/reports/generate", GenerateReport)
            .WithName("GenerateReport")
            .WithSummary("Generate assessment report")
            .WithDescription("Generates a comprehensive assessment report");
    }

    private static async Task<IResult> ReceivePackage(
        Guid shipmentId,
        [FromBody] ReceivePackageRequest request,
        IAssessmentService assessmentService,
        CancellationToken cancellationToken)
    {
        try
        {
            var assessmentId = await assessmentService.ReceivePackageAsync(
                shipmentId,
                request.QuoteId,
                request.CustomerId,
                request.TrackingNumber,
                request.DeviceType,
                request.DeviceBrand,
                request.DeviceModel,
                request.OriginalQuoteValue,
                request.ExpectedCondition,
                request.ReceivedBy,
                request.ReceivingLocation,
                cancellationToken);

            return Results.Created($"/api/assessments/{assessmentId}", new { AssessmentId = assessmentId });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
    }

    private static async Task<IResult> GetAssessment(
        Guid id,
        IAssessmentService assessmentService,
        CancellationToken cancellationToken)
    {
        var assessment = await assessmentService.GetAssessmentAsync(id, cancellationToken);
        return assessment != null ? Results.Ok(assessment) : Results.NotFound();
    }

    private static async Task<IResult> GetCustomerAssessments(
        Guid customerId,
        IAssessmentService assessmentService,
        CancellationToken cancellationToken)
    {
        var assessments = await assessmentService.GetCustomerAssessmentsAsync(customerId, cancellationToken);
        return assessments != null ? Results.Ok(assessments) : Results.NotFound();
    }

    private static async Task<IResult> GetInspectionCriteria(
        string deviceType,
        IAssessmentService assessmentService,
        CancellationToken cancellationToken)
    {
        var criteria = await assessmentService.GetInspectionCriteriaAsync(deviceType, cancellationToken);
        return Results.Ok(criteria);
    }

    private static async Task<IResult> StartInspection(
        Guid id,
        [FromBody] StartInspectionRequest request,
        IAssessmentService assessmentService,
        CancellationToken cancellationToken)
    {
        try
        {
            await assessmentService.StartInspectionAsync(
                id,
                request.InspectorId,
                request.InspectorName,
                cancellationToken);

            return Results.Ok(new { Message = "Inspection started successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
    }

    private static async Task<IResult> CompleteInspection(
        Guid id,
        [FromBody] CompleteInspectionRequest request,
        IAssessmentService assessmentService,
        CancellationToken cancellationToken)
    {
        try
        {
            await assessmentService.CompleteInspectionAsync(
                id,
                request.InspectionResults,
                request.ActualCondition,
                request.Issues,
                request.Photos,
                request.Notes,
                cancellationToken);

            return Results.Ok(new { Message = "Inspection completed and device classified successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
    }

    private static async Task<IResult> GenerateReport(
        Guid id,
        IAssessmentService assessmentService,
        CancellationToken cancellationToken)
    {
        try
        {
            await assessmentService.GenerateReportAsync(id, cancellationToken);
            return Results.Ok(new { Message = "Assessment report generated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { Error = ex.Message });
        }
    }
}

public record ReceivePackageRequest(
    Guid QuoteId,
    Guid CustomerId,
    string TrackingNumber,
    string DeviceType,
    string DeviceBrand,
    string DeviceModel,
    decimal OriginalQuoteValue,
    string ExpectedCondition,
    string ReceivedBy,
    string ReceivingLocation);

public record StartInspectionRequest(
    string InspectorId,
    string InspectorName);

public record CompleteInspectionRequest(
    Dictionary<string, object> InspectionResults,
    string ActualCondition,
    List<string> Issues,
    List<string> Photos,
    string Notes);