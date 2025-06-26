using Quote.API.Infrastructure.Config;
using Quote.API.Infrastructure.Messaging;
using Quote.API.Application.Commands;
using Quote.API.Application.Queries;
using Quote.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddMartenSetup();
builder.AddMessaging();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add application services
builder.Services.AddScoped<CreateQuote.Handler>();
builder.Services.AddScoped<ProvideQuote.Handler>();
builder.Services.AddScoped<GetQuote.Handler>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapQuoteEndpoints();

app.Run();
