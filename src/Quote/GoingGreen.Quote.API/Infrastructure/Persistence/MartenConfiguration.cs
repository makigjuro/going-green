using Marten;

namespace Quote.API.Infrastructure.Persistence;

public static class MartenConfiguration
{
    public static void ConfigureForQuote(this StoreOptions options)
    {
        // Additional Marten configuration or projections can be placed here.
        // Currently we rely on runtime aggregation of QuoteAggregate.
    }
}
