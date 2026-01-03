# Vera - Quick Start Guide

Get the Vera dating app running in 5 minutes with .NET Aspire!

## ?? Prerequisites Checklist

Before you start, make sure you have:

- [ ] .NET 10 SDK installed
- [ ] Docker Desktop running
- [ ] Visual Studio 2025 or VS Code with C# Dev Kit

## ?? Quick Start (5 Minutes)

### Step 1: Install .NET Aspire Workload (1 minute)

```bash
dotnet workload install aspire
```

### Step 2: Clone and Build (2 minutes)

```bash
git clone https://github.com/davidpizon/Vera.git
cd Vera
dotnet restore
dotnet build
```

### Step 3: Run with Aspire (2 minutes)

```bash
cd src/Vera.AppHost
dotnet run
```

**That's it!** The Aspire dashboard will open automatically, and you'll see:
- ? Cosmos DB Emulator running in Docker
- ? Vera API running on https://localhost:5001
- ? Aspire Dashboard showing logs, traces, and metrics

## ?? Running the Mobile App

After the backend is running:

```bash
# In a new terminal
cd src/Vera.BlazorHybrid

# For Android
dotnet build -t:Run -f net10.0-android

# For iOS (macOS only)
dotnet build -t:Run -f net10.0-ios
```

## ?? Initial Configuration (Optional)

For full functionality, configure these services:

### Option 1: Use Demo Mode (No Azure Required)
The app works out of the box with:
- Cosmos DB Emulator (started automatically by Aspire)
- Mock authentication (for testing)
- Simulated AI responses

### Option 2: Connect to Azure Services

Set up user secrets for the AppHost:

```bash
cd src/Vera.AppHost

# Azure AD
dotnet user-secrets set "AzureAd:TenantId" "your-tenant-id"
dotnet user-secrets set "AzureAd:ClientId" "your-client-id"

# Azure OpenAI
dotnet user-secrets set "ConnectionStrings:azureopenai" "Endpoint=https://your-openai.openai.azure.com/;Key=your-key"

# Encryption (generate a secure key)
dotnet user-secrets set "Encryption:Key" "$(openssl rand -base64 32)"
```

## ??? Aspire Dashboard Overview

When you run the AppHost, the Aspire Dashboard opens automatically. Here's what you'll see:

### Resources Tab
- **cosmosdb** - Cosmos DB Emulator container status
- **vera-api** - Backend API status and endpoints

### Logs Tab
- Real-time structured logs from all services
- Filter by service, level, or text

### Traces Tab
- Distributed traces showing request flows
- Performance bottlenecks highlighted

### Metrics Tab
- HTTP request metrics
- Response times and error rates
- Resource utilization

## ?? Verify Everything is Working

### 1. Check API Health

Open in browser: https://localhost:5001/health

Expected response: `Healthy`

### 2. Check Swagger UI

Open in browser: https://localhost:5001/swagger

You should see the API documentation.

### 3. Check Cosmos DB Emulator

Open in browser: https://localhost:8081/_explorer/index.html

You should see the Cosmos DB Data Explorer.

## ?? Stopping the Application

Press `Ctrl+C` in the terminal running the AppHost. This will:
- Stop the API
- Stop the Cosmos DB emulator (data persists)
- Close the Aspire Dashboard

To stop and remove the Cosmos DB container:

```bash
docker compose down
```

## ?? Next Steps

Now that everything is running:

1. **Explore the API** - Try out endpoints in Swagger UI
2. **View Telemetry** - Check logs and traces in the Aspire Dashboard
3. **Configure Azure Services** - Set up real Azure AD, OpenAI, and Cosmos DB
4. **Read the Docs** - See [ASPIRE_SETUP.md](ASPIRE_SETUP.md) for detailed information
5. **Deploy to Azure** - Follow [AZURE_DEPLOYMENT.md](AZURE_DEPLOYMENT.md) for production deployment

## ?? Troubleshooting

### "Docker daemon is not running"
- Start Docker Desktop

### "Port 5001 already in use"
- Edit `src/Vera.AppHost/Program.cs` to change the port
- Or stop the conflicting service

### "Cannot connect to Cosmos DB"
- Ensure Docker is running
- Check if port 8081 is available
- Try: `docker compose restart cosmosdb`

### "Build failed"
- Run `dotnet clean` then `dotnet restore`
- Make sure .NET 10 SDK is installed

### Mobile app can't connect
- For Android emulator, use `https://10.0.2.2:5001` as base URL
- For iOS simulator, use `https://localhost:5001`
- For physical devices, use your machine's IP address

## ?? Development Tips

### Use Visual Studio
1. Open `Vera.sln`
2. Set **Vera.AppHost** as the startup project
3. Press F5
4. Everything starts automatically with debugging support

### Use VS Code
1. Open the Vera folder
2. Install C# Dev Kit extension
3. Open terminal: `cd src/Vera.AppHost && dotnet run`
4. Use the C# extension for debugging

### Watch for Changes
The API supports hot reload. Just edit code and save - changes apply automatically!

### View All Logs
In the Aspire Dashboard, use the Logs tab to see real-time logs from all services.

## ?? Learning Resources

- [.NET Aspire Docs](https://learn.microsoft.com/dotnet/aspire/)
- [ASP.NET Core Docs](https://learn.microsoft.com/aspnet/core/)
- [.NET MAUI Docs](https://learn.microsoft.com/dotnet/maui/)
- [Azure Cosmos DB Docs](https://learn.microsoft.com/azure/cosmos-db/)

---

**Happy coding!** ??

For detailed documentation, see:
- [ASPIRE_SETUP.md](ASPIRE_SETUP.md) - Complete Aspire setup
- [AZURE_DEPLOYMENT.md](AZURE_DEPLOYMENT.md) - Deploy to Azure
- [src/Vera.BlazorHybrid/ASPIRE_INTEGRATION.md](src/Vera.BlazorHybrid/ASPIRE_INTEGRATION.md) - Mobile integration
