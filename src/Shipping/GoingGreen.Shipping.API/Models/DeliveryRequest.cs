namespace Shipping.API.Models;

public record DeliveryRequest(
    string DeliveredTo,
    string DeliveryLocation);