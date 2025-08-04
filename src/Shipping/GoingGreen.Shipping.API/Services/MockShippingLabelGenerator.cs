using GoingGreen.Shipping.Application.Projections;
using GoingGreen.Shipping.Application.Services;

namespace Shipping.API.Services;

public class MockShippingLabelGenerator : IShippingLabelGenerator
{
    private static readonly string[] Carriers = { "UPS", "FedEx", "USPS", "DHL" };
    private static readonly string[] Services = { "Ground", "Express", "Overnight", "2-Day" };

    public Task<ShippingLabelInfo> GenerateLabelAsync(ShipmentProjection shipment)
    {
        var random = new Random();
        var carrier = Carriers[random.Next(Carriers.Length)];
        var service = Services[random.Next(Services.Length)];
        
        var trackingNumber = GenerateTrackingNumber(carrier);
        var cost = CalculateShippingCost(service);
        var estimatedDeliveryDate = CalculateEstimatedDelivery(service);
        var labelUrl = $"https://shipping-labels.example.com/labels/{trackingNumber}.pdf";

        var labelInfo = new ShippingLabelInfo(
            trackingNumber,
            carrier,
            service,
            labelUrl,
            cost,
            estimatedDeliveryDate);

        return Task.FromResult(labelInfo);
    }

    private static string GenerateTrackingNumber(string carrier)
    {
        var random = new Random();
        return carrier switch
        {
            "UPS" => $"1Z{random.Next(100000, 999999)}{random.Next(10000000, 99999999)}",
            "FedEx" => $"{random.Next(1000, 9999)} {random.Next(1000, 9999)} {random.Next(1000, 9999)}",
            "USPS" => $"94{random.Next(10, 99)}{random.NextInt64(1000000000L, 9999999999L)}",
            "DHL" => $"{random.NextInt64(1000000000L, 9999999999L)}",
            _ => $"TRK{random.Next(100000000, 999999999)}"
        };
    }

    private static decimal CalculateShippingCost(string service)
    {
        return service switch
        {
            "Ground" => 12.99m,
            "Express" => 24.99m,
            "2-Day" => 19.99m,
            "Overnight" => 34.99m,
            _ => 15.99m
        };
    }

    private static DateTime CalculateEstimatedDelivery(string service)
    {
        var daysToAdd = service switch
        {
            "Overnight" => 1,
            "2-Day" => 2,
            "Express" => 3,
            "Ground" => 5,
            _ => 4
        };

        return DateTime.UtcNow.AddDays(daysToAdd);
    }
}