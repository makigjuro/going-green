using GoingGreen.Shipping.Application.Services;

namespace Shipping.API.Services;

public class MockShippingCarrierNotifier : IShippingCarrierNotifier
{
    public Task<CarrierNotificationResult> NotifyCarrierAsync(ShippingLabelInfo labelInfo)
    {
        // Simulate carrier notification
        var notificationId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var status = "SUCCESS";

        var result = new CarrierNotificationResult(notificationId, status);
        return Task.FromResult(result);
    }
}