namespace GoingGreen.Shipping.Application.Domain.ValueObjects;

public enum ShippingStatus
{
    Requested,
    LabelGenerated,
    ServiceNotified,
    InTransit,
    OutForDelivery,
    Delivered,
    Cancelled,
    Lost,
    Returned
}