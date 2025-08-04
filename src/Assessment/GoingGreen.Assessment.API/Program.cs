using GoingGreen.Assessment.Application.Domain.Services;
using GoingGreen.Assessment.Application.Projections;
using GoingGreen.Assessment.Application.Services;
using GoingGreen.Assessment.API.Endpoints;
using GoingGreen.Assessment.API.IntegrationEventHandlers;
using GoingGreen.Assessment.API.Services;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddMarten(options =>
    {
        options.Connection(builder.Configuration.GetConnectionString("Database")!);
        options.Events.AddEventType<GoingGreen.Assessment.Application.Domain.Events.PackageReceived>();
        options.Events.AddEventType<GoingGreen.Assessment.Application.Domain.Events.InspectionStarted>();
        options.Events.AddEventType<GoingGreen.Assessment.Application.Domain.Events.InspectionCompleted>();
        options.Events.AddEventType<GoingGreen.Assessment.Application.Domain.Events.DeviceClassified>();
        options.Events.AddEventType<GoingGreen.Assessment.Application.Domain.Events.OfferRecalculated>();
        options.Events.AddEventType<GoingGreen.Assessment.Application.Domain.Events.AssessmentReportGenerated>();
        
        options.Projections.Add<AssessmentProjectionBuilder>(ProjectionLifecycle.Inline);
        options.Projections.Add<CustomerAssessmentsProjectionBuilder>(ProjectionLifecycle.Inline);
    });

builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<IInspectionRulesEngine, InspectionRulesEngine>();
builder.Services.AddScoped<IAssessmentReportGenerator, MockAssessmentReportGenerator>();

builder.Services.AddScoped<PackageDeliveredEventHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapAssessmentEndpoints();

app.Run();
