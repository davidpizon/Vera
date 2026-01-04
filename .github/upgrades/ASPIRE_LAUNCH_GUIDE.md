# ?? Aspire Launch Guide for Vera

## Quick Start (30 seconds)

### Step-by-Step First Launch

1. **Ensure Docker Desktop is running**
   ```sh
   docker ps
   # Should return list of containers (or empty, but no error)
   ```

2. **Navigate to AppHost**
   ```sh
   cd src/Vera.AppHost
   ```

3. **Launch Aspire**
   ```sh
   dotnet run
   ```

4. **Wait for dashboard URL** (appears in ~10-30 seconds)
   ```
   info: Aspire.Hosting.DistributedApplication[0]
         Dashboard running at: http://localhost:15888
   ```

5. **Dashboard opens automatically in your browser**
   - If not, manually navigate to the URL shown in console

---

## What You'll See

### Console Output Example

```
Building...
  Determining projects to restore...
  All projects are up-to-date for restore.
  Vera.ServiceDefaults -> C:\git\davidpizon\Vera\src\Vera.ServiceDefaults\bin\Debug\net10.0\Vera.ServiceDefaults.dll
  Vera.AppHost -> C:\git\davidpizon\Vera\src\Vera.AppHost\bin\Debug\net10.0\Vera.AppHost.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.34

info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 10.1.0
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application starting.
info: Aspire.Hosting.DistributedApplication[0]
      Listening on: http://localhost:15888
info: Aspire.Hosting.DistributedApplication[0]
      Dashboard is available at: http://localhost:15888

info: Aspire.Hosting.Docker[0]
      Starting container: cosmosdb
info: Aspire.Hosting.Docker[0]
      Starting container: storage
info: Aspire.Hosting.Resource[0]
      Starting project: vera-api
```

### Dashboard Interface

#### Resources Tab (Main View)
```
???????????????????????????????????????????????????????????
? Resource     ? Type      ? State   ? Endpoints          ?
???????????????????????????????????????????????????????????
? cosmosdb     ? Container ? Running ? https://localhost:8081 ?
? storage      ? Container ? Running ? http://localhost:10000 ?
? vera-api     ? Project   ? Running ? https://localhost:5001 ?
???????????????????????????????????????????????????????????
```

Click any resource to see:
- Environment variables
- Logs for that service
- Metrics
- Health status

#### Logs Tab
- Real-time log streaming
- Color-coded by severity (Info, Warning, Error)
- Full-text search
- Time-based filtering

#### Traces Tab
- Distributed traces showing request flow
- Timeline view of operations
- Dependency visualization

#### Metrics Tab
- HTTP request graphs
- Response time charts
- Error rate tracking

---

## Common First-Launch Scenarios

### Scenario 1: Docker Not Running

**Symptom:**
```
error: Docker daemon is not running
```

**Solution:**
1. Start Docker Desktop
2. Wait for Docker to fully start (Docker icon in system tray turns solid)
3. Run `dotnet run` again

---

### Scenario 2: Port Already in Use

**Symptom:**
```
warn: Aspire.Hosting.DistributedApplication[0]
      Port 15888 is already in use, trying next available port
info: Aspire.Hosting.DistributedApplication[0]
      Dashboard running at: http://localhost:15889
```

**Solution:**
- This is normal! Aspire automatically finds the next available port
- Use the URL shown in the console

---

### Scenario 3: Containers Downloading (First Time)

**Symptom:**
```
info: Aspire.Hosting.Docker[0]
      Pulling image: mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest
```

**What's happening:**
- First launch downloads Docker images (can take 2-10 minutes)
- Cosmos DB Emulator: ~2GB
- Azurite Storage: ~200MB

**What to do:**
- Wait patiently, this only happens once
- Images are cached for future launches

---

### Scenario 4: Everything Green ?

**What you see:**
```
info: Aspire.Hosting.DistributedApplication[0]
      Dashboard running at: http://localhost:15888
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**What to do:**
1. Open browser to `http://localhost:15888`
2. Click **Resources** tab
3. Verify all resources show "Running"
4. Click on `vera-api` ? See logs and metrics
5. Test API: `https://localhost:5001/health`

---

## Accessing Your Services

### Vera.API
- **URL:** https://localhost:5001
- **Health Check:** https://localhost:5001/health
- **Swagger UI:** https://localhost:5001/swagger

### Cosmos DB Emulator
- **Endpoint:** https://localhost:8081
- **Data Explorer:** https://localhost:8081/_explorer/index.html
- **Connection String:** Available in dashboard under `cosmosdb` resource

### Azurite Storage
- **Blob Service:** http://localhost:10000
- **Queue Service:** http://localhost:10001
- **Table Service:** http://localhost:10002

---

## Stopping Aspire

### Graceful Shutdown
In the console where `dotnet run` is running:
```
Press Ctrl+C
```

**What happens:**
- Services stop gracefully
- Containers stop (but data persists in volumes)
- Dashboard closes

### Force Stop (if needed)
```sh
# Stop all Docker containers
docker stop $(docker ps -q)
```

---

## Troubleshooting

### Dashboard Won't Open

**Try:**
1. Check console for actual URL (might not be :15888)
2. Manually navigate to URL shown in console
3. Check browser isn't blocking localhost

### API Won't Start

**Check:**
1. Port 5000/5001 available: `netstat -an | findstr :5001` (Windows) or `lsof -i :5001` (Mac/Linux)
2. Build succeeded: Look for build errors in console
3. Check Logs tab in dashboard for startup errors

### Cosmos DB Container Fails

**Check:**
1. Docker has enough memory (4GB minimum recommended)
2. Check Docker Desktop ? Settings ? Resources
3. View container logs in dashboard

---

## Development Workflow

### Typical Daily Workflow

1. **Start work:**
   ```sh
   cd src/Vera.AppHost
   dotnet run
   ```

2. **Develop:**
   - Edit code in Vera.API, Vera.Application, etc.
   - Changes hot-reload automatically (for supported changes)
   - Watch logs in Aspire Dashboard

3. **Test:**
   - Use Swagger UI: https://localhost:5001/swagger
   - Monitor requests in Traces tab
   - Check logs in Logs tab

4. **End work:**
   - Press Ctrl+C in console
   - Data persists for next session

---

## Next Steps After First Launch

### Explore the Dashboard
- Click through each tab (Resources, Logs, Traces, Metrics)
- Click on each resource to see details
- Try filtering logs by severity

### Test the API
1. Open https://localhost:5001/swagger
2. Try the health endpoint
3. Authenticate (if configured)
4. Test conversation, photo, or match endpoints

### Configure Secrets (if needed)
```sh
cd src/Vera.AppHost
dotnet user-secrets set "AzureAd:TenantId" "your-tenant-id"
dotnet user-secrets set "ConnectionStrings:azureopenai" "your-connection-string"
```

### Read More Documentation
- `ASPIRE_SETUP.md` - Detailed setup guide
- `QUICKSTART.md` - 5-minute quick start
- `ASPIRE_IMPLEMENTATION.md` - Architecture details
- `.github/upgrades/aspire-sdk-installation-report.md` - Your current setup

---

## Advanced: Visual Studio Launch Configuration

If using Visual Studio, you can customize launch:

### Properties/launchSettings.json (already configured)
```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:15888"
    }
  }
}
```

---

## Tips & Tricks

### Keep Dashboard Open
The dashboard stays open as long as `dotnet run` is running. If you close the browser tab, just navigate back to the URL shown in console.

### Multiple Terminals
You can have multiple terminals open:
- Terminal 1: Running Aspire AppHost
- Terminal 2: Running tests or other commands

### View All Running Resources
In dashboard ? Resources tab ? Click column headers to sort by State, Type, etc.

### Search Logs
Logs tab ? Use search box to find specific log messages across all services

### Trace Requests
Make a request to your API ? Traces tab ? See the full distributed trace

---

## Ready to Launch?

**Right now, run this:**
```sh
cd src/Vera.AppHost
dotnet run
```

Then watch the magic happen! ??

The Aspire Dashboard will open, and you'll see your entire application orchestrated and monitored in real-time.

**First launch might take 2-10 minutes** as Docker images download. Subsequent launches take ~10-30 seconds.

---

## Questions?

- **Dashboard not opening?** Check the console for the actual URL
- **Containers not starting?** Ensure Docker Desktop is running
- **API errors?** Check Logs tab in dashboard
- **Still stuck?** See troubleshooting section above or check existing documentation

**Happy developing with Aspire!** ??
