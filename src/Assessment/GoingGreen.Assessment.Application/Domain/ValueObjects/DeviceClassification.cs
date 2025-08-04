namespace GoingGreen.Assessment.Application.Domain.ValueObjects;

public enum DeviceClassification
{
    Resellable,
    Recycle,
    Rejected
}

public enum ResaleGrade
{
    A, // Excellent condition
    B, // Good condition  
    C, // Fair condition
    D  // Poor but functional
}

public record ClassificationResult(
    DeviceClassification Classification,
    ResaleGrade? ResaleGrade,
    decimal Value,
    string Reason)
{
    public static ClassificationResult Resellable(ResaleGrade grade, decimal value, string reason)
        => new(DeviceClassification.Resellable, grade, value, reason);

    public static ClassificationResult Recycle(decimal value, string reason)
        => new(DeviceClassification.Recycle, null, value, reason);

    public static ClassificationResult Rejected(string reason)
        => new(DeviceClassification.Rejected, null, 0, reason);
}