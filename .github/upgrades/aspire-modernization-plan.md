# .NET Aspire 10.x Workflow Modernization Plan

## Executive Summary

This plan modernizes the deprecated .NET Aspire workflow patterns in the Vera dating application from legacy emulator patterns to the current .NET Aspire 10.x publishing model. The changes align with Microsoft's latest cloud-native development practices and prepare the codebase for seamless Azure deployment.

## What's Being Modernized

### Deprecated Patterns (Current State)
1. **`RunAsEmulator()` with container configuration** - Legacy pattern
2. **`WithLifetime(ContainerLifetime.Persistent)`** - Removed API
3. **Direct emulator configuration** - No longer recommended
4. **Storage `.RunAsEmulator()`** - Superseded by publishing model

### Modern Patterns (.NET Aspire 10.x)
1. **`RunAsContainer()` for local development** - Standard containerization
2. **`PublishAsAzureCosmosDBEmulator()` for deployment** - Publishing interface
3. **`PublishAsConnectionString()` for Azure resources** - Cloud deployment
4. **Simplified resource configuration** - Cleaner syntax

## Scope of Changes

### Files to Modify
1. ? `src/Vera.AppHost/Program.cs` - Main orchestration file
2. ? `src/Vera.AppHost/Vera.AppHost.csproj` - Update package versions if needed
3. ?? Documentation updates (ASPIRE_SETUP.md, QUICKSTART.md, etc.)

### What Won't Change
- ? **Vera.API** - No changes required, already using service defaults correctly
- ? **Vera.ServiceDefaults** - Already follows best practices
- ? **Vera.Infrastructure** - Connection string handling already supports both models
- ? **All business logic** - Zero impact on domain/application layers

## Detailed Changes

### 1. Cosmos DB Resource Modernization

#### Current Code (Deprecated)
```csharp
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
    .RunAsEmulator(emulator => emulator
        .WithLifetime(ContainerLifetime.Persistent));

var database = cosmosDb.AddDatabase("VeraDb");
```

#### Modern Code (.NET Aspire 10.x)
```csharp
// For local development with emulator
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
    .RunAsContainer(container => 
    {
        container.WithDataVolume("vera-cosmosdb-data"); // Persistent data
    });

var database = cosmosDb.AddDatabase("VeraDb");
```

**Alternative for Production/Hybrid**:
```csharp
// Auto-selects emulator in dev, connection string in production
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb");
var database = cosmosDb.AddDatabase("VeraDb");
```

**Benefits:**
- ? Works with both emulator and Azure Cosmos DB
- ? Automatic environment detection
- ? Cleaner, more maintainable code
- ? Aligned with Aspire 10.x best practices

### 2. Storage Resource Modernization

#### Current Code (Deprecated)
```csharp
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();

var tables = storage.AddTables("tables");
```

#### Modern Code (.NET Aspire 10.x)
```csharp
// For local development with Azurite emulator
var storage = builder.AddAzureStorage("storage")
    .RunAsContainer();

var tables = storage.AddTables("tables");
```

**Alternative Simplified**:
```csharp
// Auto-configuration based on environment
var storage = builder.AddAzureStorage("storage");
var tables = storage.AddTables("tables");
```

**Benefits:**
- ? Azurite container automatically managed
- ? Persistent volumes handled automatically
- ? Production-ready with connection string override

### 3. Azure OpenAI (Already Correct ?)

Your current implementation is already using the modern pattern:
```csharp
var openai = builder.AddConnectionString("azureopenai");
```

**No changes needed** - this follows Aspire 10.x best practices.

## Migration Steps

### Step 1: Backup Current Configuration
```bash
# Create backup
git checkout -b backup/aspire-legacy
git commit -am "Backup before Aspire modernization"
git checkout main
```

### Step 2: Update AppHost Program.cs

Replace the resource configuration section with modern patterns:

```csharp
namespace Vera.AppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        // Modern pattern: Azure Cosmos DB with automatic emulator/cloud switching
        var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
            .RunAsContainer(container =>
            {
                container.WithDataVolume("vera-cosmosdb-data");
            });

        var database = cosmosDb.AddDatabase("VeraDb");

        // Modern pattern: Azure Storage with Azurite emulator
        var storage = builder.AddAzureStorage("storage")
            .RunAsContainer();
        
        var tables = storage.AddTables("tables");

        // Modern pattern: Azure OpenAI connection (already correct)
        var openai = builder.AddConnectionString("azureopenai");

        // Add the API with service discovery
        var api = builder.AddProject<Projects.Vera_API>("vera-api")
            .WithReference(database)
            .WithReference(tables)
            .WithEnvironment("AzureOpenAI__ConnectionString", openai)
            .WithExternalHttpEndpoints();

        builder.Build().Run();
    }
}
```

### Step 3: Update Package References (if needed)

Check and update to latest stable Aspire packages:

```xml
<ItemGroup>
  <PackageReference Include="Aspire.Hosting.AppHost" Version="10.1.0" />
  <PackageReference Include="Aspire.Hosting.Azure.CosmosDB" Version="10.1.0" />
  <PackageReference Include="Aspire.Hosting.Azure.Storage" Version="10.1.0" />
</ItemGroup>
```

Your current versions (10.1.0) are already correct ?

### Step 4: Test Local Development

```bash
cd src/Vera.AppHost
dotnet run
```

Verify:
- ? Cosmos DB emulator starts in container
- ? Azurite storage emulator starts
- ? Vera.API connects successfully
- ? Aspire Dashboard shows all resources
- ? Data persists between restarts (volume mounting)

### Step 5: Test Azure Deployment

```bash
cd src/Vera.AppHost
azd up
```

Verify:
- ? Azure resources provision correctly
- ? Connection strings auto-configured
- ? Application deploys successfully
- ? Telemetry flows to Azure Monitor

### Step 6: Update Documentation

Update the following files to reflect modern patterns:

#### ASPIRE_SETUP.md
- Remove references to `RunAsEmulator()` and `WithLifetime()`
- Update code examples to show `RunAsContainer()`
- Clarify automatic environment switching

#### QUICKSTART.md
- Update quick start commands (no changes needed, already correct)
- Update troubleshooting for new container patterns

#### ASPIRE_IMPLEMENTATION.md
- Update "What Was Implemented" section
- Document new resource configuration patterns
- Update package versions to 10.1.0

## Environment Behavior

### Local Development (Default)
When running `dotnet run` in AppHost:
- ? Cosmos DB Emulator runs in Docker container
- ? Azurite Storage Emulator runs in Docker container
- ? Data persists in Docker volumes
- ? Services use `http://localhost` endpoints

### Azure Deployment (via `azd up`)
When deploying to Azure:
- ? Azure Cosmos DB provisioned (or uses existing via connection string)
- ? Azure Storage Account provisioned (or uses existing)
- ? Connection strings injected automatically
- ? Managed Identity configured (if enabled)

## Breaking Changes Assessment

### For Developers
- ? **No workflow changes** - `dotnet run` still works the same way
- ? **No configuration changes** - User secrets remain the same
- ? **No new prerequisites** - Docker still required, same as before

### For CI/CD
- ? **No pipeline changes** - Build and deployment work identically
- ? **No deployment changes** - `azd up` works exactly the same

### For API/Services
- ? **Zero impact** - Connection strings injected the same way
- ? **Zero code changes** - Infrastructure layer already supports both patterns

## Validation Checklist

After migration, verify:

- [ ] Local development works
  - [ ] `cd src/Vera.AppHost && dotnet run`
  - [ ] Aspire Dashboard opens
  - [ ] Cosmos DB container running
  - [ ] Storage container running
  - [ ] API accessible at https://localhost:5001
  - [ ] Health checks pass (/health endpoint)

- [ ] Data persistence works
  - [ ] Create test data in Cosmos DB
  - [ ] Stop AppHost
  - [ ] Restart AppHost
  - [ ] Verify data still exists

- [ ] Azure deployment works
  - [ ] `azd up` provisions resources
  - [ ] Application deploys successfully
  - [ ] Health checks pass in Azure
  - [ ] Telemetry visible in Azure Monitor

- [ ] Documentation updated
  - [ ] ASPIRE_SETUP.md reflects new patterns
  - [ ] Code examples updated
  - [ ] Troubleshooting guide current

## Rollback Plan

If issues arise, rollback is simple:

```bash
# Restore previous version
git checkout backup/aspire-legacy src/Vera.AppHost/Program.cs

# Rebuild and restart
cd src/Vera.AppHost
dotnet run
```

Alternatively, keep both patterns side-by-side temporarily:

```csharp
// Can mix old and new approaches during transition
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb");

// For local dev only:
if (builder.Environment.IsDevelopment())
{
    cosmosDb.RunAsContainer(c => c.WithDataVolume("vera-cosmosdb-data"));
}
```

## Timeline

| Phase | Duration | Tasks |
|-------|----------|-------|
| **Phase 1: Code Update** | 30 minutes | Update Program.cs, test locally |
| **Phase 2: Documentation** | 1 hour | Update all Aspire documentation |
| **Phase 3: Validation** | 30 minutes | Test deployment to Azure |
| **Phase 4: Commit & Deploy** | 15 minutes | Commit changes, update main branch |

**Total Estimated Time:** ~2.25 hours

## Benefits Summary

### Immediate Benefits
- ? **Compliance** - Uses current .NET Aspire 10.x APIs
- ? **Cleaner Code** - Simplified resource configuration
- ? **Better Defaults** - Automatic environment detection
- ? **Future-Proof** - Aligned with Microsoft roadmap

### Long-Term Benefits
- ? **Easier Maintenance** - Modern patterns are better documented
- ? **Better Tooling Support** - IDEs understand new APIs better
- ? **Performance** - New container orchestration is optimized
- ? **Cloud-Native** - Seamless local-to-cloud transition

## Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| Local dev breaks | Low | Medium | Test thoroughly before committing |
| Azure deployment fails | Low | High | Test `azd up` in dev subscription first |
| Data loss | Very Low | High | Volumes persist automatically, backup before migration |
| Breaking API changes | Very Low | Medium | Aspire 10.x is stable, no further changes expected |

## Success Criteria

Migration is successful when:
- ? Local development works with `dotnet run`
- ? Data persists between restarts
- ? Aspire Dashboard shows all resources correctly
- ? Azure deployment works with `azd up`
- ? All health checks pass
- ? Documentation reflects new patterns
- ? No warnings or deprecation messages in build output

## Next Steps After Migration

Once modernization is complete:

1. **Monitoring Enhancement**
   - Add custom metrics to Aspire Dashboard
   - Configure Azure Monitor alerts
   - Set up Application Insights queries

2. **Resource Optimization**
   - Review Cosmos DB throughput settings
   - Configure autoscaling rules
   - Optimize container resource limits

3. **CI/CD Integration**
   - Generate GitHub Actions workflow via `azd pipeline config`
   - Add environment-specific configurations
   - Set up staging/production environments

## References

- [.NET Aspire 10.x Release Notes](https://learn.microsoft.com/dotnet/aspire/whats-new/aspire-10)
- [Aspire Azure Cosmos DB Integration](https://learn.microsoft.com/dotnet/aspire/database/azure-cosmos-db)
- [Aspire Azure Storage Integration](https://learn.microsoft.com/dotnet/aspire/storage/azure-storage)
- [Azure Developer CLI Documentation](https://learn.microsoft.com/azure/developer/azure-developer-cli/)

---

**Ready to begin migration?** Start with Step 1 and proceed through the validation checklist.
