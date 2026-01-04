# Aspire Quick Start Guide

## TL;DR - Get Started in 2 Minutes

```bash
# 1. Install Aspire workload (one-time setup)
dotnet workload install aspire

# 2. Start the application
cd src/Vera.AppHost
dotnet run

# 3. Open the dashboard URL shown in console
# Example: https://localhost:17285
```

That's it! All services (API, Cosmos DB, Azure Storage) start automatically.

## What You Get

- ?? **Aspire Dashboard** - Visual interface for logs, traces, and metrics
- ?? **Vera.API** - Running at `https://localhost:5001`
- ?? **Cosmos DB Emulator** - Auto-started in Docker
- ?? **Azure Storage Emulator** - Auto-started in Docker
- ?? **OpenTelemetry** - Distributed tracing and metrics
- ?? **Service Discovery** - Services find each other automatically

## Dashboard Features

### Logs Tab
- Real-time logs from all services
- Filter by service, level, or search text
- Click to expand and see full details

### Traces Tab
- See request flows across services
- Identify slow operations
- Debug complex workflows

### Metrics Tab
- HTTP request rates
- Response times
- Error rates
- Database performance

### Resources Tab
- Service health status
- Running containers
- Environment variables
- Connection strings

## Common Tasks

### View API Logs
1. Open Aspire Dashboard
2. Click "Logs" tab
3. Filter by "vera-api"
4. See real-time log output

### Check Cosmos DB Connection
1. Open Aspire Dashboard
2. Click "Resources" tab
3. Find "cosmosdb" resource
4. Check status is "Running"
5. View connection string in "Environment" section

### Debug a Request
1. Make API request (e.g., via Swagger at `https://localhost:5001/swagger`)
2. Open "Traces" tab in Dashboard
3. Find your request
4. Click to see detailed trace
5. See timing breakdown and any errors

### Configure Azure OpenAI
```bash
cd src/Vera.AppHost
dotnet user-secrets set "ConnectionStrings:azureopenai" "Endpoint=https://YOUR-RESOURCE.openai.azure.com/;Key=YOUR-KEY"
```

## Development Workflow

```bash
# Start everything
cd src/Vera.AppHost
dotnet run

# Make code changes to API
# ? Hot reload applies automatically
# ? See updates in Dashboard logs

# Stop everything
# ? Press Ctrl+C in AppHost console
# ? All services stop
```

## Running Without Aspire

If you need to run just the API:

```bash
cd src/Vera.API
dotnet run
```

This uses traditional configuration from `appsettings.Development.json`.

## Troubleshooting

### "Docker is not running"
- **Option 1**: Start Docker Desktop
- **Option 2**: Aspire will show warning but API still runs (without emulators)

### "Port already in use"
- Stop other instances of the application
- Check Aspire Dashboard isn't already running
- Kill process using the port: `netstat -ano | findstr :5001`

### "Certificate not trusted"
```bash
dotnet dev-certs https --trust
```

### "Aspire workload not found"
```bash
dotnet workload install aspire
```

## VS Code

Add to `.vscode/tasks.json`:

```json
{
  "label": "aspire-run",
  "command": "dotnet",
  "type": "process",
  "args": ["run", "--project", "${workspaceFolder}/src/Vera.AppHost"],
  "problemMatcher": "$msCompile"
}
```

Press `Ctrl+Shift+B` ? Select "aspire-run"

## Visual Studio

1. Open `Vera.sln`
2. Right-click `Vera.AppHost` project
3. Select "Set as Startup Project"
4. Press `F5` to run with debugging
5. Dashboard opens automatically

## Deploy to Azure

```bash
# One-time setup
winget install microsoft.azd
cd src/Vera.AppHost
azd init

# Deploy (creates all Azure resources)
azd up

# Access your deployed app
azd show
```

## Key Endpoints

| Service | URL | Description |
|---------|-----|-------------|
| Aspire Dashboard | `https://localhost:17285` | Monitoring and logs |
| Vera API | `https://localhost:5001` | REST API |
| Swagger UI | `https://localhost:5001/swagger` | API documentation |
| Health Check | `https://localhost:5001/health` | Health status |
| Cosmos DB Emulator | `https://localhost:8081/_explorer` | Database UI |

## Learn More

- ?? [Full Aspire Configuration Guide](ASPIRE_COORDINATION.md)
- ?? [.NET Aspire Docs](https://learn.microsoft.com/dotnet/aspire/)
- ?? [Azure Deployment Guide](../src/Vera.BlazorHybrid/ASPIRE_INTEGRATION.md)

---

**Happy coding!** ??
