# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Going Green is a .NET 9 microservices application showcasing modern architecture patterns with Azure Container Apps deployment. It implements Neal Ford's Going Green architecture kata with event-driven microservices, Infrastructure as Code (Terraform), and containerized deployment.

## Architecture

The solution follows a microservices architecture with:

- **AppHost**: .NET Aspire orchestration project (`apphost/GoingGreen.AppHost/`) that defines the local development environment
- **Microservices**: Five domain services under `src/`:
  - Customer API (`src/Customer/GoingGreen.Customer.API/`)
  - Device Registry API (`src/DeviceRegistry/GoingGreen.DeviceRegistry.API/`)
  - Payment API (`src/Payment/GoingGreen.Payment.API/`)
  - Quote API (`src/Quote/GoingGreen.Quote.API/`)
  - Shipping API (`src/Shipping/GoingGreen.Shipping.API/`)
- **Gateway**: API Gateway (`src/Gateway/GoingGreen.Gateway/`)
- **Common**: Shared service defaults (`src/Common/GoingGreen.ServiceDefaults/`)
- **Infrastructure**: Terraform modules in `infra/terraform/`

## Development Commands

### Running the Application
```bash
# Run the entire application stack with .NET Aspire
cd apphost/GoingGreen.AppHost
dotnet run
```

### Building
```bash
# Build entire solution
dotnet build GoingGreen.sln

# Build specific service
dotnet build src/Customer/GoingGreen.Customer.API/GoingGreen.Customer.API.csproj
```

### Testing Individual APIs
Use the `.http` files in each API project for testing endpoints:
- `src/DeviceRegistry/GoingGreen.DeviceRegistry.API/DeviceRegistry.API.http`
- `src/Payment/GoingGreen.Payment.API/Payment.API.http`
- `src/Quote/GoingGreen.Quote.API/Quote.API.http`
- `src/Shipping/GoingGreen.Shipping.API/Shipping.API.http`

### Infrastructure
```bash
# Deploy infrastructure
cd infra/terraform
terraform init
terraform plan
terraform apply
```

## Key Technologies & Patterns

- **Event-Driven Architecture**: Each service has an `Eventing/` folder with EventingExtensions.cs and Events.cs
- **Event Store**: Marten with PostgreSQL for event sourcing
- **Messaging**: Azure Service Bus (emulated locally) for inter-service communication
- **Database**: PostgreSQL with Marten ORM
- **Observability**: OpenTelemetry integration in ServiceDefaults
- **Service Discovery**: .NET Aspire service discovery
- **Resilience**: HTTP resilience patterns via Microsoft.Extensions.Http.Resilience

## Service Structure

Each microservice follows a consistent pattern:
- `Program.cs`: Entry point with `builder.AddServiceDefaults()` and `builder.AddEventing()`
- `Eventing/EventingExtensions.cs`: Marten event store and Service Bus configuration
- `Eventing/Events.cs`: Domain events (where applicable)
- `Dockerfile`: Container configuration
- `appsettings.json` & `appsettings.Development.json`: Configuration

## Local Development Environment

The AppHost (`apphost/GoingGreen.AppHost/Program.cs`) orchestrates:
- SQL Edge container for Service Bus emulator backend
- Service Bus emulator container
- PostgreSQL container with pgAdmin (port 5050)
- All microservices with proper service references

## Infrastructure Modules

Terraform modules in `infra/terraform/modules/`:
- `azure_key_vault/`: Azure Key Vault for secrets
- `azure_service_bus/`: Service Bus for messaging
- `container_app/`: Azure Container Apps
- `container_registry/`: Azure Container Registry
- `log_analytics/`: Monitoring and logging
- `postgresql/`: Azure Database for PostgreSQL

## Connection Strings & Configuration

Services expect these connection strings:
- `"Postgres"` or `POSTGRES_CONNECTION_STRING`: PostgreSQL database
- `"ServiceBus"` or `SERVICEBUS_CONNECTION_STRING`: Azure Service Bus

Local emulator connection string: `"Endpoint=sb://servicebus-emulator/;SharedAccessKeyName=admin;SharedAccessKey=Password123"`

## Important Notes

- All services target .NET 9 with nullable reference types enabled
- The solution uses Marten 8.0.0 for event sourcing
- Services communicate via events published to Service Bus topics named after the event type
- Each service maintains its own database schema via Marten auto-creation
- Infrastructure is fully defined in Terraform with modular design