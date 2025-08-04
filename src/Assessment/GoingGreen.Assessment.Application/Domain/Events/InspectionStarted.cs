namespace GoingGreen.Assessment.Application.Domain.Events;

public class InspectionStarted
{
    public Guid AssessmentId { get; init; }
    public string InspectorId { get; init; } = string.Empty;
    public string InspectorName { get; init; } = string.Empty;
    public List<string> InspectionCriteria { get; init; } = new();
    public DateTime StartedAt { get; init; }

    public InspectionStarted() { }

    public InspectionStarted(
        Guid assessmentId,
        string inspectorId,
        string inspectorName,
        List<string> inspectionCriteria,
        DateTime startedAt)
    {
        AssessmentId = assessmentId;
        InspectorId = inspectorId;
        InspectorName = inspectorName;
        InspectionCriteria = inspectionCriteria;
        StartedAt = startedAt;
    }
}