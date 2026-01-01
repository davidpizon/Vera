# Deployment Guide for Vera Dating App

This guide explains how to deploy the Vera dating application to Azure.

## Prerequisites

- Azure Subscription
- Azure CLI installed
- Docker installed (for container deployment)
- .NET 10 SDK installed

## Azure Resources Setup

### 1. Create Resource Group

```bash
az group create --name vera-rg --location eastus
```

### 2. Create Azure Cosmos DB

```bash
az cosmosdb create \
  --name vera-cosmosdb \
  --resource-group vera-rg \
  --default-consistency-level Session \
  --locations regionName=eastus failoverPriority=0 isZoneRedundant=False

# Get connection details
az cosmosdb keys list --name vera-cosmosdb --resource-group vera-rg --type connection-strings
```

### 3. Create Azure OpenAI Resource

```bash
az cognitiveservices account create \
  --name vera-openai \
  --resource-group vera-rg \
  --kind OpenAI \
  --sku S0 \
  --location eastus

# Deploy GPT-4 model
az cognitiveservices account deployment create \
  --name vera-openai \
  --resource-group vera-rg \
  --deployment-name gpt-4 \
  --model-name gpt-4 \
  --model-version "0613" \
  --model-format OpenAI \
  --sku-capacity 10 \
  --sku-name "Standard"

# Get endpoint and keys
az cognitiveservices account show --name vera-openai --resource-group vera-rg --query properties.endpoint
az cognitiveservices account keys list --name vera-openai --resource-group vera-rg
```

### 4. Set Up Microsoft Entra External ID

1. Go to Azure Portal > Microsoft Entra ID
2. Create a new App Registration:
   - Name: "Vera Dating App"
   - Supported account types: "Accounts in any organizational directory and personal Microsoft accounts"
   - Redirect URI: Add your mobile app URIs
3. Configure Authentication:
   - Add platform: Mobile and desktop applications
   - Add redirect URI: `msal://auth`
4. Add Google/Facebook identity providers:
   - Go to External Identities > Identity providers
   - Add Google and Facebook providers
5. Configure API permissions:
   - Add delegated permissions
6. Create API scope:
   - Go to Expose an API
   - Add scope: `access_as_user`
7. Note down:
   - Tenant ID
   - Client ID

### 5. Create Azure Container Registry (Optional)

```bash
az acr create \
  --resource-group vera-rg \
  --name veraacr \
  --sku Basic

az acr login --name veraacr
```

## Backend Deployment

### Option 1: Azure Container Apps (Recommended)

```bash
# Build and push Docker image
docker build -t vera-api:latest .
docker tag vera-api:latest veraacr.azurecr.io/vera-api:latest
docker push veraacr.azurecr.io/vera-api:latest

# Create Container Apps environment
az containerapp env create \
  --name vera-env \
  --resource-group vera-rg \
  --location eastus

# Deploy container app
az containerapp create \
  --name vera-api \
  --resource-group vera-rg \
  --environment vera-env \
  --image veraacr.azurecr.io/vera-api:latest \
  --target-port 8080 \
  --ingress external \
  --env-vars \
    "AzureAd__TenantId=YOUR_TENANT_ID" \
    "AzureAd__ClientId=YOUR_CLIENT_ID" \
    "CosmosDb__Endpoint=YOUR_COSMOS_ENDPOINT" \
    "CosmosDb__Key=YOUR_COSMOS_KEY" \
    "AzureOpenAI__Endpoint=YOUR_OPENAI_ENDPOINT" \
    "AzureOpenAI__ApiKey=YOUR_OPENAI_KEY" \
    "Encryption__Key=YOUR_ENCRYPTION_KEY"

# Get app URL
az containerapp show --name vera-api --resource-group vera-rg --query properties.configuration.ingress.fqdn
```

### Option 2: Azure App Service

```bash
# Create App Service Plan
az appservice plan create \
  --name vera-plan \
  --resource-group vera-rg \
  --sku P1V2 \
  --is-linux

# Create Web App
az webapp create \
  --name vera-api \
  --resource-group vera-rg \
  --plan vera-plan \
  --runtime "DOTNETCORE:10.0"

# Configure app settings
az webapp config appsettings set \
  --name vera-api \
  --resource-group vera-rg \
  --settings \
    AzureAd__TenantId=YOUR_TENANT_ID \
    AzureAd__ClientId=YOUR_CLIENT_ID \
    CosmosDb__Endpoint=YOUR_COSMOS_ENDPOINT \
    CosmosDb__Key=YOUR_COSMOS_KEY \
    AzureOpenAI__Endpoint=YOUR_OPENAI_ENDPOINT \
    AzureOpenAI__ApiKey=YOUR_OPENAI_KEY \
    Encryption__Key=YOUR_ENCRYPTION_KEY

# Deploy code
cd src/Vera.API
dotnet publish -c Release -o ./publish
cd publish
zip -r ../app.zip .
az webapp deploy --name vera-api --resource-group vera-rg --src-path ../app.zip
```

## Mobile App Deployment

### iOS Deployment (macOS required)

1. Install MAUI workload:
   ```bash
   dotnet workload install maui
   ```

2. Update `Vera.BlazorHybrid.csproj` to enable iOS:
   ```xml
   <TargetFrameworks>net10.0-ios;net10.0-android</TargetFrameworks>
   ```

3. Configure app settings:
   - Update API URL in `MauiProgram.cs`
   - Update authentication settings in `AuthenticationService.cs`

4. Build for iOS:
   ```bash
   cd src/Vera.BlazorHybrid
   dotnet build -f net10.0-ios -c Release
   ```

5. Deploy to App Store:
   - Archive app in Xcode
   - Upload to App Store Connect
   - Submit for review

### Android Deployment

1. Build for Android:
   ```bash
   cd src/Vera.BlazorHybrid
   dotnet build -f net10.0-android -c Release
   ```

2. Sign the APK:
   ```bash
   dotnet publish -f net10.0-android -c Release /p:AndroidKeyStore=true \
     /p:AndroidSigningKeyStore=mykey.keystore \
     /p:AndroidSigningStorePass=password \
     /p:AndroidSigningKeyAlias=mykey \
     /p:AndroidSigningKeyPass=password
   ```

3. Deploy to Google Play:
   - Upload APK/AAB to Google Play Console
   - Create release
   - Submit for review

## Environment Variables

Create a `.env` file or set environment variables:

```bash
# Azure AD
export AZURE_AD_TENANT_ID="your-tenant-id"
export AZURE_AD_CLIENT_ID="your-client-id"

# Cosmos DB
export COSMOS_DB_ENDPOINT="https://your-account.documents.azure.com:443/"
export COSMOS_DB_KEY="your-cosmos-key"

# Azure OpenAI
export AZURE_OPENAI_ENDPOINT="https://your-openai.openai.azure.com/"
export AZURE_OPENAI_API_KEY="your-openai-key"

# Encryption
export ENCRYPTION_KEY="your-32-character-encryption-key"
```

## Security Considerations

1. **Never commit secrets** to source control
2. **Use Azure Key Vault** for production secrets:
   ```bash
   az keyvault create --name vera-kv --resource-group vera-rg --location eastus
   az keyvault secret set --vault-name vera-kv --name CosmosDbKey --value "YOUR_KEY"
   ```
3. **Enable HTTPS only** on all endpoints
4. **Configure CORS** properly in production
5. **Enable Application Insights** for monitoring:
   ```bash
   az monitor app-insights component create \
     --app vera-insights \
     --location eastus \
     --resource-group vera-rg
   ```

## CI/CD with GitHub Actions

The repository includes GitHub Actions workflows. To enable:

1. Add secrets to GitHub repository:
   - `AZURE_CREDENTIALS` (Service Principal)
   - `ACR_USERNAME`
   - `ACR_PASSWORD`
   - `AZURE_WEBAPP_NAME`

2. Workflows will automatically:
   - Build and test on PR
   - Scan dependencies
   - Deploy to Azure on merge to main

## Monitoring and Logging

1. **Enable Application Insights** in `Program.cs`:
   ```csharp
   builder.Services.AddApplicationInsightsTelemetry();
   ```

2. **Configure Log Analytics**:
   ```bash
   az monitor log-analytics workspace create \
     --resource-group vera-rg \
     --workspace-name vera-logs
   ```

3. **Set up alerts** for critical errors and performance issues

## Scaling

### Backend Scaling

- **Container Apps**: Automatically scales based on HTTP traffic
- **App Service**: Configure scale rules in Azure Portal

### Database Scaling

- **Cosmos DB**: Configure throughput (RU/s) based on usage
- Enable autoscale for automatic adjustment

## Cost Optimization

1. Use **Azure Reservations** for predictable workloads
2. Configure **autoscaling** to scale down during low traffic
3. Use **Consumption tier** for Container Apps for low-traffic periods
4. Monitor costs with **Azure Cost Management**

## Troubleshooting

### Common Issues

1. **Authentication fails**:
   - Verify Tenant ID and Client ID
   - Check redirect URIs match exactly

2. **Cosmos DB connection issues**:
   - Verify endpoint and key
   - Check firewall rules

3. **OpenAI errors**:
   - Ensure model is deployed
   - Verify API key and endpoint
   - Check quota limits

### Logs

View logs:
```bash
# Container Apps
az containerapp logs show --name vera-api --resource-group vera-rg --follow

# App Service
az webapp log tail --name vera-api --resource-group vera-rg
```

## Support

For issues or questions:
- Create an issue in the GitHub repository
- Check Azure documentation: https://docs.microsoft.com/azure
