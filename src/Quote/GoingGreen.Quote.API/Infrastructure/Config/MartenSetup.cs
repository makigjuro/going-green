using Marten;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quote.API.Domain;
using Quote.API.Domain.Events;
using Weasel.Core;

namespace Quote.API.Infrastructure.Config;

public static class MartenSetup
{
    public static IHostApplicationBuilder AddMartenSetup(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var pg = configuration.GetConnectionString("Postgres") ?? configuration["POSTGRES_CONNECTION_STRING"];
        builder.Services.AddMarten(opts =>
            {
                opts.Connection(pg);
                opts.AutoCreateSchemaObjects = AutoCreate.All;
                opts.Projections.SelfAggregate<QuoteAggregate>();
            })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        return builder;
    }
}
