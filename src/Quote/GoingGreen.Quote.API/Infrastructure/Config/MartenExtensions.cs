using JasperFx;
using Marten;
using Marten.Events.Projections;
using Quote.API.Domain;

namespace Quote.API.Infrastructure.Config;

public static class MartenExtensions
{
    public static IHostApplicationBuilder AddMartenSetup(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var pg = configuration.GetConnectionString("Postgres") ?? configuration["POSTGRES_CONNECTION_STRING"];
        builder.Services.AddMarten(opts =>
            {
                opts.Connection(pg);
                opts.AutoCreateSchemaObjects = AutoCreate.All;
                opts.Projections.Snapshot<QuoteAggregate>(SnapshotLifecycle.Inline);
            })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        return builder;
    }
}
