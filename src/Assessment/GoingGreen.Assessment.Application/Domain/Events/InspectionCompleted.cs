namespace GoingGreen.Assessment.Application.Domain.Events;

public class InspectionCompleted
{
    public Guid AssessmentId { get; init; }
    public string InspectorId { get; init; } = string.Empty;
    public Dictionary<string, object> InspectionResults { get; init; } = new();
    public string ActualCondition { get; init; } = string.Empty;
    public List<string> Issues { get; init; } = new();
    public List<string> Photos { get; init; } = new();
    public string Notes { get; init; } = string.Empty;
    public DateTime CompletedAt { get; init; }

    public InspectionCompleted() { }

    public InspectionCompleted(
        Guid assessmentId,
        string inspectorId,
        Dictionary<string, object> inspectionResults,
        string actualCondition,
        List<string> issues,
        List<string> photos,
        string notes,
        DateTime completedAt)
    {
        AssessmentId = assessmentId;
        InspectorId = inspectorId;
        InspectionResults = inspectionResults;
        ActualCondition = actualCondition;
        Issues = issues;
        Photos = photos;
        Notes = notes;
        CompletedAt = completedAt;
    }
}