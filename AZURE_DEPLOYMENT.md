# Azure Deployment with .NET Aspire

This guide explains how to deploy the Vera application to Azure using .NET Aspire's deployment features.

## Prerequisites

- Azure subscription
- Azure Developer CLI (`azd`) installed
- Docker installed
- .NET 10 SDK with Aspire workload

## Install Azure Developer CLI

### Windows
```powershell
winget install microsoft.azd
```

### macOS
```bash
brew install azd
```

### Linux
```bash
curl -fsSL https://aka.ms/install-azd.sh | bash
```

## Initialize Azure Deployment

1. **Navigate to the AppHost directory**:
   ```bash
   cd src/Vera.AppHost
   ```

2. **Initialize azd**:
   ```bash
   azd init
   ```
   
   When prompted:
   - Environment name: `vera-dev` (or your preferred name)
   - Location: Choose your preferred Azure region (e.g., `eastus2`)

3. **Review generated files**:
   - `azure.yaml` - Defines Azure resources
   - `infra/` folder - Contains Bicep templates for infrastructure

## Configure Azure Resources

The Aspire AppHost automatically generates infrastructure as code. Review and customize:

### Main Resources
- **Azure Container Apps** - Hosts the API
- **Azure Cosmos DB** - Database (or uses existing account)
- **Azure Container Registry** - Stores container images
- **Azure Log Analytics** - Centralized logging
- **Azure Monitor** - Application insights

## Deploy to Azure

1. **Login to Azure**:
   ```bash
   azd auth login
   ```

2. **Provision and deploy**:
   ```bash
   azd up
   ```
   
   This command will:
   - Create Azure resources
   - Build container images
   - Push images to Azure Container Registry
   - Deploy to Azure Container Apps
   - Configure environment variables
   - Set up managed identities

3. **Monitor deployment**:
   - Follow the progress in the terminal
   - View resources in the Azure Portal
   - Check the deployment logs

## Environment Configuration

### Set Azure AD Configuration

```bash
azd env set AZURE_AD_TENANT_ID "your-tenant-id"
azd env set AZURE_AD_CLIENT_ID "your-client-id"
azd env set AZURE_AD_CLIENT_SECRET "your-client-secret"
```

### Set Encryption Key

```bash
azd env set ENCRYPTION_KEY "your-secure-encryption-key"
```

### Set Azure OpenAI Configuration

```bash
azd env set AZURE_OPENAI_ENDPOINT "https://your-openai.openai.azure.com/"
azd env set AZURE_OPENAI_KEY "your-openai-key"
azd env set AZURE_OPENAI_DEPLOYMENT "gpt-4"
```

## Use Existing Azure Resources

### Existing Cosmos DB

To use an existing Cosmos DB account instead of creating a new one:

1. Update `src/Vera.AppHost/Program.cs`:
   ```csharp
   var cosmosDb = builder.AddConnectionString("cosmosdb");
   ```

2. Set the connection string:
   ```bash
   azd env set COSMOSDB_CONNECTIONSTRING "your-connection-string"
   ```

### Existing Azure OpenAI

Set the connection string for your existing Azure OpenAI:

```bash
azd env set AZURE_OPENAI_CONNECTIONSTRING "Endpoint=https://your-openai.openai.azure.com/;Key=your-key"
```

## Managed Identity (Recommended for Production)

For production, use managed identities instead of connection strings:

1. **Update Program.cs** to use DefaultAzureCredential:
   ```csharp
   builder.AddAzureCosmosClient("cosmosdb", 
       configureClientBuilder: clientBuilder => 
       {
           clientBuilder.WithCredentials(new DefaultAzureCredential());
       });
   ```

2. **Grant permissions** via Azure Portal or CLI:
   ```bash
   # Get the managed identity principal ID
   PRINCIPAL_ID=$(az containerapp show \
       --name vera-api \
       --resource-group rg-vera-dev \
       --query identity.principalId -o tsv)
   
   # Grant Cosmos DB access
   az cosmosdb sql role assignment create \
       --account-name cosmos-vera-dev \
       --resource-group rg-vera-dev \
       --scope "/" \
       --principal-id $PRINCIPAL_ID \
       --role-definition-name "Cosmos DB Built-in Data Contributor"
   ```

## Monitor the Deployed Application

### View Logs

```bash
azd monitor --logs
```

### View Metrics

```bash
azd monitor --overview
```

### Azure Portal

1. Navigate to the resource group in Azure Portal
2. Select the Container App
3. View:
   - Metrics
   - Log Analytics
   - Application Insights
   - Container logs

## Update the Deployment

After making code changes:

```bash
azd deploy
```

This rebuilds and redeploys only the changed services.

## Mobile App Configuration

After deployment, update the mobile app configuration:

1. **Get the API URL**:
   ```bash
   azd env get-values
   ```
   
   Look for the API endpoint (e.g., `https://vera-api.azurecontainerapps.io`)

2. **Update mobile app settings**:
   
   In `src/Vera.BlazorHybrid/appsettings.json`:
   ```json
   {
     "ApiSettings": {
       "BaseUrl": "https://vera-api.azurecontainerapps.io"
     }
   }
   ```

3. **Rebuild and republish** the mobile app

## CI/CD with GitHub Actions

Aspire can generate GitHub Actions workflows:

1. **Generate workflow**:
   ```bash
   azd pipeline config
   ```

2. **Commit and push**:
   ```bash
   git add .
   git commit -m "Add Azure deployment workflow"
   git push
   ```

3. **Configure GitHub secrets**:
   - `AZURE_CREDENTIALS` - Service principal credentials
   - Any other sensitive configuration

## Cost Management

### Monitor Costs

```bash
azd monitor --cost
```

### Stop Resources

To stop all resources (useful for dev environments):

```bash
azd down
```

This deletes all Azure resources but keeps your environment configuration.

### Scale Down for Development

For development environments, consider:
- Using Cosmos DB serverless tier
- Scaling Container Apps to 0 replicas when not in use
- Using Azure OpenAI pay-as-you-go pricing

## Troubleshooting

### Deployment Fails

1. Check logs:
   ```bash
   azd deploy --debug
   ```

2. Verify Azure permissions
3. Check resource quotas in your subscription
4. Review Bicep template errors

### Container App Won't Start

1. Check container logs in Azure Portal
2. Verify environment variables
3. Check managed identity permissions
4. Review health check configuration

### Connection Issues

1. Verify network security groups
2. Check Container Apps ingress configuration
3. Verify CORS settings
4. Check firewall rules

## Production Best Practices

1. **Use Managed Identities** - No connection strings in code
2. **Enable Azure Front Door** - For global distribution
3. **Configure Custom Domains** - Use your own domain
4. **Set up Monitoring** - Application Insights alerts
5. **Implement Rate Limiting** - Protect your API
6. **Use Azure Key Vault** - For secrets management
7. **Enable Backup** - For Cosmos DB
8. **Configure Auto-scaling** - Based on metrics
9. **Set up Staging Environment** - Test before production
10. **Implement Blue-Green Deployment** - Zero-downtime updates

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Azure Developer CLI Documentation](https://learn.microsoft.com/azure/developer/azure-developer-cli/)
