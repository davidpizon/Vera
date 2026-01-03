# .NET Aspire Setup for Vera

This document explains how to run the Vera application using .NET Aspire orchestration.

## Prerequisites

1. **.NET 10 SDK** - Ensure you have .NET 10 installed
2. **.NET Aspire workload** - Install using:
   ```bash
   dotnet workload install aspire
   ```
3. **Docker Desktop** - Required for running the Cosmos DB emulator
4. **Visual Studio 2025** or **Visual Studio Code** with C# Dev Kit

## Project Structure

The Aspire orchestration includes:

- **Vera.AppHost** - Aspire orchestration project that manages all services
- **Vera.ServiceDefaults** - Shared configuration for observability and resilience
- **Vera.API** - Backend API with Aspire integration
- **Vera.BlazorHybrid** - Mobile frontend (connects to API via service discovery)

## Running the Application

### Using Visual Studio

1. Set **Vera.AppHost** as the startup project
2. Press F5 to run
3. The Aspire dashboard will open automatically at http://localhost:15888 (or similar)

### Using Command Line

```bash
cd src/Vera.AppHost
dotnet run
```

### What Gets Started

When you run the AppHost:

1. **Cosmos DB Emulator** - Runs in Docker container (persistent)
2. **Vera.API** - Backend API on http://localhost:5000 and https://localhost:5001
3. **Aspire Dashboard** - Monitoring dashboard showing all services, logs, traces, and metrics

## Aspire Dashboard Features

Access the dashboard at the URL shown in the console output. Features include:

- **Resources** - View all running services and their status
- **Logs** - Structured logging from all services
- **Traces** - Distributed tracing across services
- **Metrics** - Performance metrics and charts
- **Environment Variables** - Configuration for each service

## Connecting the Mobile App

The Blazor Hybrid mobile app should connect to the API using:

### Development (Aspire)
- **Base URL**: `https://localhost:5001` or use service discovery
- The API URL is exposed and can be discovered via Aspire

### Production
- Configure the appropriate production API URL in the mobile app settings

## Configuration

### User Secrets for AppHost

Store sensitive configuration in user secrets:

```bash
cd src/Vera.AppHost
dotnet user-secrets set "AzureAd:TenantId" "YOUR_TENANT_ID"
dotnet user-secrets set "AzureAd:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "ConnectionStrings:azureopenai" "YOUR_AZURE_OPENAI_CONNECTION_STRING"
```

### Cosmos DB

The orchestration uses the Cosmos DB Emulator by default. To use Azure Cosmos DB:

1. Update `src/Vera.AppHost/Program.cs`:
   ```csharp
   var cosmosDb = builder.AddConnectionString("cosmosdb");
   ```

2. Add connection string to user secrets:
   ```bash
   dotnet user-secrets set "ConnectionStrings:cosmosdb" "YOUR_COSMOS_DB_CONNECTION_STRING"
   ```

## Health Checks

The API exposes health check endpoints:

- `/health` - Overall health status
- `/alive` - Liveness probe (for Kubernetes)

## Observability

Aspire provides built-in observability:

- **OpenTelemetry** - Distributed tracing and metrics
- **Structured Logging** - Centralized log aggregation
- **Service Discovery** - Automatic service endpoint resolution

## Troubleshooting

### Cosmos DB Emulator Issues

If the Cosmos DB emulator fails to start:

1. Ensure Docker Desktop is running
2. Pull the latest emulator image:
   ```bash
   docker pull mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest
   ```
3. Check Docker logs in the Aspire dashboard

### Port Conflicts

If ports 5000/5001 are in use, update the port configuration in `src/Vera.AppHost/Program.cs`:

```csharp
api.WithHttpEndpoint(port: 5500, name: "http")
   .WithHttpsEndpoint(port: 5501, name: "https");
```

## Mobile Development Workflow

1. Start the AppHost to run the backend API
2. Note the API URL from the Aspire dashboard
3. Configure the Blazor Hybrid app to use this URL
4. Run the mobile app separately using your preferred mobile development workflow

## Next Steps

- Configure Azure Entra ID for authentication
- Set up Azure OpenAI connection string
- Deploy to Azure using `azd` (Azure Developer CLI) - Aspire generates deployment templates
- Add more services (e.g., Azure Storage, Redis) to the orchestration as needed
