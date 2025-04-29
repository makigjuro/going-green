var builder = DistributedApplication.CreateBuilder(args);



var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres:15-alpine")
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050))
    .WithDataVolume(isReadOnly: false)
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "password");

var goingGreenDb = postgres.AddDatabase("going-green-db");

var customerService = builder.AddProject<Projects.GoingGreen_Customer_API>("customerservice")
    .WithReference(goingGreenDb);
var paymentService = builder.AddProject<Projects.GoingGreen_Payment_API>("paymentservice")
    .WithReference(goingGreenDb);
var quoteService = builder.AddProject<Projects.GoingGreen_Quote_API>("quoteservice")
    .WithReference(goingGreenDb);
var shippingService = builder.AddProject<Projects.GoingGreen_Shipping_API>("shippingservice")
    .WithReference(goingGreenDb);
var deviceRegistryService = builder.AddProject<Projects.GoingGreen_DeviceRegistry_API>("deviceregistryservice")
    .WithReference(goingGreenDb);

builder.Build().Run();
