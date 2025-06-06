var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Edge container
var sqlEdge = builder.AddContainer("sqledge", "mcr.microsoft.com/azure-sql-edge:latest")
    .WithEnvironment("ACCEPT_EULA", Environment.GetEnvironmentVariable("ACCEPT_EULA") ?? "Y")
    .WithEnvironment("MSSQL_SA_PASSWORD", Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? "YourStrong!Passw0rd")
    .WithEndpoint(name: "sql", port: 1433, targetPort: 1433, isExternal: false); // Default SQL port, used internally

var configPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "ServiceBusConfig.json"));

//Add Service Bus Emulator container
var sbEmulator = builder.AddContainer("servicebus-emulator", "mcr.microsoft.com/azure-messaging/servicebus-emulator:latest")
    .WithEnvironment("SQL_SERVER", "sqledge")
    .WithEnvironment("MSSQL_SA_PASSWORD", Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? "YourStrong!Passw0rd")
    .WithEnvironment("ACCEPT_EULA", Environment.GetEnvironmentVariable("ACCEPT_EULA") ?? "Y")
    .WithEnvironment("SQL_WAIT_INTERVAL", Environment.GetEnvironmentVariable("SQL_WAIT_INTERVAL") ?? "15")
    // Mount your local config (replace with your actual path or set in env)
    .WithBindMount(configPath, "/ServiceBus_Emulator/ServiceBusConfig.json")
    .WithEndpoint(name: "amqp", targetPort:5672,  port: 5672, isExternal: true) // AMQP
    .WithEndpoint(name: "mrmt" , targetPort:5300,  port: 5300, isExternal: true) // Management/Control
    .WaitForCompletion(sqlEdge);

var serviceBusConnectionString = "Endpoint=sb://servicebus-emulator/;SharedAccessKeyName=admin;SharedAccessKey=Password123";

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres:15-alpine")
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050))
    .WithDataVolume(isReadOnly: false)
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "password");

var goingGreenDb = postgres.AddDatabase("going-green-db");

var customerService = builder.AddProject<Projects.GoingGreen_Customer_API>("customer-api")
    .WithReference(goingGreenDb)
    .WithEnvironment("ServiceBus__ConnectionString", serviceBusConnectionString);

var paymentService = builder.AddProject<Projects.GoingGreen_Payment_API>("payment-api")
    .WithReference(goingGreenDb)
    .WithEnvironment("ServiceBus__ConnectionString", serviceBusConnectionString);

var quoteService = builder.AddProject<Projects.GoingGreen_Quote_API>("quote-api")
    .WithReference(goingGreenDb)
    .WithEnvironment("ServiceBus__ConnectionString", serviceBusConnectionString);

var shippingService = builder.AddProject<Projects.GoingGreen_Shipping_API>("shipping-api")
    .WithReference(goingGreenDb)
    .WithEnvironment("ServiceBus__ConnectionString", serviceBusConnectionString);

var deviceRegistryService = builder.AddProject<Projects.GoingGreen_DeviceRegistry_API>("device-registry-api")
    .WithReference(goingGreenDb)
    .WithEnvironment("ServiceBus__ConnectionString", serviceBusConnectionString);

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
