var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres:15-alpine")
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050))
    .WithDataVolume(isReadOnly: false)
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "password");

var goingGreenDb = postgres.AddDatabase("going-green-db");

var customerService = builder.AddProject<Projects.GoingGreen_Customer_API>("customer-api")
    .WithReference(goingGreenDb);

var paymentService = builder.AddProject<Projects.GoingGreen_Payment_API>("payment-api")
    .WithReference(goingGreenDb);

var quoteService = builder.AddProject<Projects.GoingGreen_Quote_API>("quote-api")
    .WithReference(goingGreenDb);

var shippingService = builder.AddProject<Projects.GoingGreen_Shipping_API>("shipping-api")
    .WithReference(goingGreenDb);

var deviceRegistryService = builder.AddProject<Projects.GoingGreen_DeviceRegistry_API>("device-registry-api")
    .WithReference(goingGreenDb);

var gateway = builder.AddProject<Projects.GoingGreen_Gateway>("gateway")
    .WithReference(customerService)
    .WithReference(paymentService)
    .WithReference(quoteService)
    .WithReference(shippingService)
    .WithReference(deviceRegistryService)
    .WaitFor(customerService)
    .WaitFor(paymentService)
    .WaitFor(quoteService)
    .WaitFor(shippingService)
    .WaitFor(deviceRegistryService)
    .WithExternalHttpEndpoints();

builder.Build().Run();
