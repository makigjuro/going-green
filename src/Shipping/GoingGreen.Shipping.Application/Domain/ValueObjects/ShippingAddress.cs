namespace GoingGreen.Shipping.Application.Domain.ValueObjects;

public record ShippingAddress(
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country)
{
    public static ShippingAddress Create(
        string street, 
        string city, 
        string state, 
        string postalCode, 
        string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be empty", nameof(state));
        
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));
        
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        return new ShippingAddress(street, city, state, postalCode, country);
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {PostalCode}, {Country}";
    }
}