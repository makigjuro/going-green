using GoingGreen.Quote.Application.Domain.Events;
using GoingGreen.Quote.Application.Domain.ValueObjects;

namespace GoingGreen.Quote.Application.Domain.Aggregates;

public class Quote
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string DeviceType { get; private set; } = string.Empty;
    public string DeviceCondition { get; private set; } = string.Empty;
    public string DeviceBrand { get; private set; } = string.Empty;
    public string DeviceModel { get; private set; } = string.Empty;
    public int DeviceAge { get; private set; }
    public QuoteStatus Status { get; private set; }
    public decimal? EstimatedValue { get; private set; }
    public string? QuoteReason { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? CalculatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }

    private Quote() { }

    public static Quote RequestQuote(
        Guid quoteId,
        Guid customerId,
        string deviceType,
        string deviceCondition,
        string deviceBrand,
        string deviceModel,
        int deviceAge,
        DateTime requestedAt)
    {
        var quote = new Quote
        {
            Id = quoteId,
            CustomerId = customerId,
            DeviceType = deviceType,
            DeviceCondition = deviceCondition,
            DeviceBrand = deviceBrand,
            DeviceModel = deviceModel,
            DeviceAge = deviceAge,
            Status = QuoteStatus.Requested,
            RequestedAt = requestedAt
        };

        return quote;
    }

    public void OnDeviceValidated(bool isValid, string validationMessage, decimal baseValue, DateTime validatedAt)
    {
        if (Status != QuoteStatus.Requested && Status != QuoteStatus.DeviceValidationPending)
            throw new InvalidOperationException($"Cannot validate device for quote in status {Status}");

        if (isValid)
        {
            Status = QuoteStatus.DeviceValidated;
            CalculateQuote(baseValue, validatedAt);
        }
        else
        {
            Status = QuoteStatus.Rejected;
            RejectionReason = validationMessage;
            RejectedAt = validatedAt;
        }
    }

    private void CalculateQuote(decimal baseValue, DateTime calculatedAt)
    {
        var conditionMultiplier = DeviceCondition.ToLowerInvariant() switch
        {
            "excellent" => 1.0m,
            "good" => 0.8m,
            "fair" => 0.6m,
            "poor" => 0.4m,
            _ => 0.5m
        };

        var ageMultiplier = DeviceAge switch
        {
            <= 1 => 1.0m,
            <= 2 => 0.9m,
            <= 3 => 0.8m,
            <= 5 => 0.7m,
            <= 7 => 0.6m,
            <= 10 => 0.5m,
            _ => 0.3m
        };

        EstimatedValue = Math.Round(baseValue * conditionMultiplier * ageMultiplier, 2);
        QuoteReason = $"Base value: ${baseValue}, Condition factor: {conditionMultiplier:P0}, Age factor: {ageMultiplier:P0}";
        Status = QuoteStatus.Calculated;
        CalculatedAt = calculatedAt;
        ExpiresAt = calculatedAt.AddDays(7); // Quote valid for 7 days
    }

    public void Accept(DateTime acceptedAt)
    {
        if (Status != QuoteStatus.Calculated)
            throw new InvalidOperationException($"Cannot accept quote in status {Status}");

        if (ExpiresAt.HasValue && acceptedAt > ExpiresAt.Value)
            throw new InvalidOperationException("Cannot accept an expired quote");

        Status = QuoteStatus.Accepted;
        AcceptedAt = acceptedAt;
    }

    public void Reject(string rejectionReason, DateTime rejectedAt)
    {
        if (Status == QuoteStatus.Accepted || Status == QuoteStatus.Expired)
            throw new InvalidOperationException($"Cannot reject quote in status {Status}");

        Status = QuoteStatus.Rejected;
        RejectionReason = rejectionReason;
        RejectedAt = rejectedAt;
    }

    public void Expire(DateTime expiredAt)
    {
        if (Status != QuoteStatus.Calculated)
            throw new InvalidOperationException($"Cannot expire quote in status {Status}");

        Status = QuoteStatus.Expired;
        ExpiredAt = expiredAt;
    }
}