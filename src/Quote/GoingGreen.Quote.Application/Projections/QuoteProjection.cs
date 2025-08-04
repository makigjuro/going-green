using GoingGreen.Quote.Application.Domain.Events;
using GoingGreen.Quote.Application.Domain.ValueObjects;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace GoingGreen.Quote.Application.Projections;

public class QuoteProjection
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string DeviceCondition { get; set; } = string.Empty;
    public string DeviceBrand { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public int DeviceAge { get; set; }
    public QuoteStatus Status { get; set; }
    public decimal? EstimatedValue { get; set; }
    public string? QuoteReason { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CalculatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
}

public class QuoteProjectionBuilder : SingleStreamProjection<QuoteProjection, Guid>
{
    public QuoteProjection Create(QuoteRequested @event)
    {
        return new QuoteProjection
        {
            Id = @event.QuoteId,
            CustomerId = @event.CustomerId,
            DeviceType = @event.DeviceType,
            DeviceCondition = @event.DeviceCondition,
            DeviceBrand = @event.DeviceBrand,
            DeviceModel = @event.DeviceModel,
            DeviceAge = @event.DeviceAge,
            Status = QuoteStatus.Requested,
            RequestedAt = @event.RequestedAt
        };
    }

    public void Apply(DeviceValidated @event, QuoteProjection projection)
    {
        if (@event.IsValid)
        {
            projection.Status = QuoteStatus.DeviceValidated;
        }
        else
        {
            projection.Status = QuoteStatus.Rejected;
            projection.RejectionReason = @event.ValidationMessage;
            projection.RejectedAt = @event.ValidatedAt;
        }
    }

    public void Apply(QuoteCalculated @event, QuoteProjection projection)
    {
        projection.Status = QuoteStatus.Calculated;
        projection.EstimatedValue = @event.EstimatedValue;
        projection.QuoteReason = @event.QuoteReason;
        projection.CalculatedAt = @event.CalculatedAt;
        projection.ExpiresAt = @event.ExpiresAt;
    }

    public void Apply(QuoteRejected @event, QuoteProjection projection)
    {
        projection.Status = QuoteStatus.Rejected;
        projection.RejectionReason = @event.RejectionReason;
        projection.RejectedAt = @event.RejectedAt;
    }

    public void Apply(QuoteAccepted @event, QuoteProjection projection)
    {
        projection.Status = QuoteStatus.Accepted;
        projection.AcceptedAt = @event.AcceptedAt;
    }

    public void Apply(QuoteExpired @event, QuoteProjection projection)
    {
        projection.Status = QuoteStatus.Expired;
        projection.ExpiredAt = @event.ExpiredAt;
    }
}