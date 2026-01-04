# Aspire Coordination Configuration Guide

This document explains how the Vera application is configured to use .NET Aspire for orchestration and coordination.

## Overview

The Vera application uses .NET Aspire to orchestrate multiple services and resources:
- **Vera.API** - REST API backend
- **Cosmos DB** - Database (runs in emulator for development)
- **Azure Table Storage** - Additional storage (runs in emulator for development)
- **Azure OpenAI** - AI conversation service

## Project Structure

### Aspire Projects

#### Vera.AppHost
The orchestration host that manages all services and resources.

**Location**: `src/Vera.AppHost/`

**Key responsibilities**:
- Start and coordinate all services
- Inject connection strings
- Configure service discovery
- Host the Aspire Dashboard

**Configuration** (`Program.cs`):
```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Cosmos DB with emulator
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
    .RunAsEmulator(emulator => emulator.WithLifetime(ContainerLifetime.Persistent));
var database = cosmosDb.AddDatabase("VeraDb");

// Azure Storage with emulator
var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var tables = storage.AddTables("tables");

// Azure OpenAI connection string
var openai = builder.AddConnectionString("azureopenai");

// API with service references
var api = builder.AddProject<Projects.Vera_API>("vera-api")
    .WithReference(database)
    .WithReference(tables)
    .WithEnvironment("AzureOpenAI__ConnectionString", openai)
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

#### Vera.ServiceDefaults
Shared configuration library for all Aspire-enabled services.

**Location**: `src/Vera.ServiceDefaults/`

**Key features**:
- OpenTelemetry configuration (logs, metrics, traces)
- Service discovery setup
- HTTP client resilience patterns
- Health checks
- Standard endpoint mapping

**Usage in API**:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
builder.AddServiceDefaults();

// ... register services ...

var app = builder.Build();

// Map health check endpoints
app.MapDefaultEndpoints();
```

## Running with Aspire

### Prerequisites
1. Install .NET 10 SDK
2. Install Aspire workload:
   ```bash
   dotnet workload install aspire
   ```
3. (Optional) Install Docker Desktop for container emulators

### Start the Application

1. **Navigate to AppHost**:
   ```bash
   cd src/Vera.AppHost
   ```

2. **Run the AppHost**:
   ```bash
   dotnet run
   ```

3. **Access the Aspire Dashboard**:
   - The dashboard URL will be displayed in the console
   - Default: `https://localhost:17285` or `http://localhost:15041`
   - The dashboard provides:
     - Real-time logs from all services
     - Distributed traces across requests
     - Metrics and performance data
     - Resource health status
     - Environment variables and configuration

### What Happens When You Run AppHost

1. **Aspire Dashboard starts** - Web UI for monitoring
2. **Cosmos DB Emulator starts** - In Docker container (if Docker is running)
3. **Azure Storage Emulator starts** - In Docker container (if Docker is running)
4. **Vera.API starts** - With auto-injected connection strings
5. **Service Discovery configured** - Services can discover each other
6. **OpenTelemetry enabled** - Logs, traces, and metrics collected

## Configuration

### Connection Strings

Aspire automatically injects connection strings as environment variables:

- `ConnectionStrings__cosmosdb` - Cosmos DB connection
- `ConnectionStrings__azureopenai` - Azure OpenAI connection
- `ConnectionStrings__tables` - Azure Table Storage connection

### User Secrets (Development)

For sensitive configuration during development:

```bash
cd src/Vera.AppHost
dotnet user-secrets set "ConnectionStrings:azureopenai" "Endpoint=https://YOUR-RESOURCE.openai.azure.com/;Key=YOUR-API-KEY"
```

### Environment Variables (Production)

In Azure Container Apps (deployed via `azd`):
- Connection strings are automatically configured
- Managed identities are used for secure access
- No secrets needed in configuration files

## Service Integration

### Cosmos DB Integration

The `CosmosDbContext` supports both Aspire and traditional configuration:

```csharp
// Aspire injects: ConnectionStrings__cosmosdb
var connectionString = configuration.GetConnectionString("cosmosdb");

if (!string.IsNullOrEmpty(connectionString))
{
    _client = new CosmosClient(connectionString);
}
else
{
    // Fallback to traditional config
    var endpoint = configuration["CosmosDb:Endpoint"];
    var key = configuration["CosmosDb:Key"];
    _client = new CosmosClient(endpoint, key);
}
```

### Azure OpenAI Integration

The `AzureOpenAIConversationService` supports both formats:

```csharp
// Aspire injects: ConnectionStrings__azureopenai
var connectionString = configuration.GetConnectionString("azureopenai");

if (!string.IsNullOrEmpty(connectionString))
{
    // Parse "Endpoint=...;Key=..." format
    // Extract endpoint and key
}
else
{
    // Fallback to traditional config
    var endpoint = configuration["AzureOpenAI:Endpoint"];
    var apiKey = configuration["AzureOpenAI:ApiKey"];
}
```

## Observability Features

### Structured Logging
All logs are collected and viewable in the Aspire Dashboard:
- Filterable by service
- Searchable
- Real-time updates

### Distributed Tracing
Request flows across services are visualized:
- See how requests move between API, database, and external services
- Identify performance bottlenecks
- Debug complex workflows

### Metrics
Performance metrics automatically collected:
- HTTP request duration
- Request rates
- Error rates
- Database query performance
- Runtime metrics (GC, memory, threads)

### Health Checks
Automatic health monitoring:
- `/health` - Overall health check
- `/alive` - Liveness probe (for Kubernetes)

## Resilience Patterns

Aspire automatically configures resilience patterns for HTTP clients:

- **Retry policies** - Automatic retry on transient failures
- **Circuit breakers** - Prevent cascading failures
- **Timeouts** - Prevent hanging requests
- **Bulkhead isolation** - Limit concurrent operations

Configuration is automatic via `AddStandardResilienceHandler()` in ServiceDefaults.

## Service Discovery

Services can discover each other by name:

```csharp
// In API, call another service by its Aspire name
var httpClient = httpClientFactory.CreateClient();
var response = await httpClient.GetAsync("http://vera-api/endpoint");
```

## Development Workflow

### Local Development

1. **Start AppHost** - Runs all services
2. **Make code changes** - Hot reload supported
3. **View logs in Dashboard** - Real-time feedback
4. **Debug in Visual Studio** - Attach to specific service
5. **Stop AppHost** - Stops all services

### Running Individual Services

You can still run services independently without Aspire:

```bash
cd src/Vera.API
dotnet run
```

This uses traditional configuration from `appsettings.json`.

## Deployment

### Azure Deployment with Azure Developer CLI

1. **Install azd**:
   ```bash
   winget install microsoft.azd
   ```

2. **Initialize**:
   ```bash
   cd src/Vera.AppHost
   azd init
   ```

3. **Deploy to Azure**:
   ```bash
   azd up
   ```

This creates:
- Azure Container Apps for API
- Azure Cosmos DB account
- Azure Storage account
- Application Insights
- All necessary networking and identity

### CI/CD with GitHub Actions

The `azd` tool can generate GitHub Actions workflows:

```bash
azd pipeline config
```

This creates `.github/workflows/azure-dev.yml` for automated deployment.

## Troubleshooting

### Dashboard Not Opening
- Check console output for the correct URL
- Ensure no firewall is blocking the ports
- Try the HTTP URL if HTTPS has certificate issues

### Cosmos DB Emulator Not Starting
- Ensure Docker Desktop is running
- Check Docker has enough resources (4GB+ RAM recommended)
- View container logs in Docker Desktop
- Fallback: Use traditional config with local emulator

### Connection String Not Injected
- Check AppHost Program.cs has `.WithReference()` calls
- Verify service names match in AppHost and code
- Check Aspire Dashboard "Environment" tab for injected variables

### Build Errors
```bash
# Restore Aspire workload
dotnet workload install aspire

# Clean and rebuild
dotnet clean
dotnet build
```

## Package Versions

Current Aspire packages (ensure consistency):

```xml
<!-- AppHost -->
<PackageReference Include="Aspire.Hosting.AppHost" Version="10.1.0" />
<PackageReference Include="Aspire.Hosting.Azure.CosmosDB" Version="10.1.0" />
<PackageReference Include="Aspire.Hosting.Azure.Storage" Version="10.1.0" />

<!-- ServiceDefaults -->
<PackageReference Include="Microsoft.Extensions.Http.Resilience" Version="10.1.0" />
<PackageReference Include="Microsoft.Extensions.ServiceDiscovery" Version="10.1.0" />
<PackageReference Include="OpenTelemetry.*" Version="1.14.0" />

<!-- API -->
<PackageReference Include="Aspire.Azure.Data.Tables" Version="13.1.0" />
<PackageReference Include="Microsoft.Extensions.ServiceDiscovery" Version="10.1.0" />
```

## Benefits Summary

### For Development
- ? One command starts entire application
- ? Visual dashboard for logs and traces
- ? No manual configuration of connection strings
- ? Hot reload support
- ? Easy debugging

### For Production
- ? Azure deployment with single command
- ? Managed identities for security
- ? Automatic scaling
- ? Built-in monitoring
- ? Infrastructure as Code

### For Observability
- ? Centralized logging
- ? Distributed tracing
- ? Performance metrics
- ? Health monitoring
- ? Real-time insights

## Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire Dashboard](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)

## Next Steps

1. ? **Basic Setup** - AppHost and ServiceDefaults configured
2. ? **API Integration** - API uses service defaults
3. ? **Database Integration** - Cosmos DB with Aspire
4. ?? **Configure Azure OpenAI** - Set connection string in user secrets
5. ?? **Test Mobile App** - Connect BlazorHybrid to API
6. ?? **Deploy to Azure** - Use `azd up` for cloud deployment

---

**The Vera application is now fully configured for Aspire coordination!** ??

All services are orchestrated through the AppHost, with automatic service discovery, observability, and deployment capabilities.
