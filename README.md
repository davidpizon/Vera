# Vera - AI-Powered Dating Application

Vera is a modern, cross-platform dating application built with .NET 10, featuring AI-powered conversational profile creation, intelligent matching algorithms, secure cloud infrastructure, and **.NET Aspire orchestration** for cloud-native development.

## 🌟 Features

- **AI Conversational Assistant**: Natural language profile creation using Azure OpenAI
- **Smart Matching Algorithm**: Advanced compatibility scoring based on conversation data
- **Photo Upload with AI Feedback**: Intelligent photo analysis and recommendations
- **Cross-Platform Mobile App**: Native iOS and Android support via .NET MAUI Blazor Hybrid
- **Secure Authentication**: Microsoft Entra External ID with Google/Facebook login
- **Cloud-Native Architecture**: Azure Cosmos DB for global scale and performance
- **Data Encryption**: End-to-end encryption for sensitive user data
- **Docker Support**: Containerized backend for easy deployment
- **.NET Aspire Orchestration**: Service discovery, observability, and resilience

## 🏗️ Architecture

The application follows Clean Architecture principles with clear separation of concerns:

```
Vera/
├── src/
│   ├── Vera.Domain/          # Domain entities and interfaces
│   ├── Vera.Application/     # Business logic and use cases
│   ├── Vera.Infrastructure/  # External services (Cosmos DB, Azure OpenAI)
│   ├── Vera.API/            # REST API backend (.NET 10)
│   ├── Vera.BlazorHybrid/   # Mobile app (iOS & Android)
│   ├── Vera.AppHost/        # .NET Aspire orchestration
│   └── Vera.ServiceDefaults/# Shared Aspire configuration
├── .github/
│   ├── workflows/           # CI/CD pipelines
│   └── dependabot.yml      # Dependency scanning
├── Dockerfile              # API containerization
└── docker-compose.yml      # Local development setup
```

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- .NET Aspire workload: `dotnet workload install aspire`
- Azure Subscription (for Cosmos DB, OpenAI, and Entra ID)
- Visual Studio 2025 or VS Code
- Docker Desktop (for containerization and Cosmos DB emulator)

### Quick Start with .NET Aspire (Recommended)

1. Clone the repository:
```bash
git clone https://github.com/davidpizon/Vera.git
cd Vera
```

2. Start the Aspire AppHost:
```bash
cd src/Vera.AppHost
dotnet run
```

3. Access the Aspire Dashboard (URL shown in console, typically http://localhost:15888)
   - View all services, logs, traces, and metrics
   - Monitor health and performance

4. The API will be available at:
   - HTTPS: https://localhost:5001
   - HTTP: http://localhost:5000

For detailed Aspire setup instructions, see [ASPIRE_SETUP.md](ASPIRE_SETUP.md)

### Alternative: Traditional Local Development

#### Using Docker Compose

```bash
docker compose up -d
```

This starts the Cosmos DB emulator. See the [Cosmos DB Configuration](#cosmos-db-configuration) section below.

### Azure Services Setup

1. **Azure Cosmos DB**
   - Create a Cosmos DB account with SQL API
   - Note the endpoint and primary key

2. **Azure OpenAI**
   - Deploy an Azure OpenAI resource
   - Deploy GPT-4 model
   - Note the endpoint and API key

3. **Microsoft Entra External ID**
   - Create an app registration
   - Configure authentication with Google/Facebook
   - Add API permissions
   - Note the Tenant ID and Client ID

### Configuration

1. Clone the repository:
```bash
git clone https://github.com/davidpizon/Vera.git
cd Vera
```

2. Copy the example environment file:
```bash
cp .env.example .env
```

3. Update `.env` with your Azure credentials

4. Update `src/Vera.API/appsettings.json`:
```json
{
  "AzureAd": {
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID"
  },
  "CosmosDb": {
    "Endpoint": "YOUR_COSMOS_ENDPOINT",
    "Key": "YOUR_COSMOS_KEY"
  },
  "AzureOpenAI": {
    "Endpoint": "YOUR_OPENAI_ENDPOINT",
    "ApiKey": "YOUR_OPENAI_KEY"
  }
}
```

### Running Locally

#### Backend API

```bash
cd src/Vera.API
dotnet restore
dotnet run
```

The API will be available at `https://localhost:5001`

#### Mobile App

```bash
cd src/Vera.BlazorHybrid
dotnet restore
dotnet build
```

For Android:
```bash
dotnet build -t:Run -f net10.0-android
```

For iOS (macOS only):
```bash
dotnet build -t:Run -f net10.0-ios
```

### Docker Deployment

Build and run using Docker:

```bash
docker-compose up --build
```

Or build the image manually:

```bash
docker build -t vera-api .
docker run -p 8080:8080 vera-api
```

## 📱 Mobile App Configuration

The mobile app connects to the API backend orchestrated by Aspire.

Update the API endpoint in `src/Vera.BlazorHybrid/appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5001"
  }
}
```

For platform-specific configuration (Android emulator, iOS simulator, physical devices), see [src/Vera.BlazorHybrid/ASPIRE_INTEGRATION.md](src/Vera.BlazorHybrid/ASPIRE_INTEGRATION.md)

Update authentication settings in `src/Vera.BlazorHybrid/Services/AuthenticationService.cs`:

```csharp
_msalClient = PublicClientApplicationBuilder
    .Create("YOUR_CLIENT_ID")
    .WithAuthority("https://login.microsoftonline.com/YOUR_TENANT_ID")
    .Build();
```

## 🔍 Observability with .NET Aspire

The Aspire dashboard provides comprehensive observability:

- **Real-time Logs**: Centralized logging from all services
- **Distributed Tracing**: End-to-end request tracing with OpenTelemetry
- **Metrics**: Performance metrics and charts
- **Resource Monitoring**: Health checks and service status
- **Service Discovery**: Automatic endpoint resolution

Access the dashboard when running the AppHost - the URL will be displayed in the console.

## 🔒 Security Features

- **Authentication**: Microsoft Entra External ID with OAuth 2.0
- **Authorization**: JWT Bearer tokens with role-based access
- **Data Encryption**: AES encryption for sensitive data
- **HTTPS**: TLS/SSL for all communications
- **Dependency Scanning**: Weekly Dependabot scans + PR validation
- **Container Security**: Trivy scanning for Docker images

## 🧪 Testing

Run tests:

```bash
dotnet test
```

## 📊 CI/CD

The project includes GitHub Actions workflows for:

- **Build & Test**: Automated builds on PR and push
- **Dependency Scanning**: Weekly NuGet, Docker, and GitHub Actions scans + PR validation
- **Security Scanning**: Automated vulnerability detection on every PR

## ☁️ Deployment

### Deploy to Azure with Aspire

Using Azure Developer CLI (`azd`):

```bash
cd src/Vera.AppHost
azd init
azd up
```

This automatically:
- Creates Azure resources (Container Apps, Cosmos DB, etc.)
- Builds and pushes container images
- Deploys the application
- Configures observability

For detailed deployment instructions, see [AZURE_DEPLOYMENT.md](AZURE_DEPLOYMENT.md)

### Traditional Docker Deployment

```bash
docker-compose up --build
```

Or build the image manually:

```bash
docker build -t vera-api .
docker run -p 8080:8080 vera-api
```

## 🛠️ Technology Stack

### Backend
- .NET 10.0
- ASP.NET Core Web API
- Azure Cosmos DB
- Azure OpenAI (GPT-4)
- Microsoft Identity Web
- **.NET Aspire** - Cloud-native orchestration

### Mobile
- .NET MAUI
- Blazor Hybrid
- iOS (14.2+)
- Android (API 26+)

### Infrastructure
- Azure App Service / Container Apps
- Azure Cosmos DB
- Azure OpenAI
- Microsoft Entra External ID
- Docker
- **.NET Aspire** - Service orchestration and observability

## 📝 API Endpoints

- `POST /api/conversation/chat` - Send message to AI assistant
- `GET /api/conversation` - Get conversation history
- `POST /api/photo/upload` - Upload profile photo
- `GET /api/photo` - Get user photos
- `POST /api/match/generate` - Generate matches
- `GET /api/match` - Get user matches
- `POST /api/match/{id}/interest` - Express interest in match

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 🔗 Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/)
- [Azure OpenAI](https://learn.microsoft.com/azure/ai-services/openai/)
- [Microsoft Entra External ID](https://learn.microsoft.com/entra/external-id/)

## 📧 Support

For issues and questions, please create an issue in the GitHub repository.

# Vera
Conversational assisted mobile dating application

## Table of Contents
- [Prerequisites](#prerequisites)
- [Local Development Setup](#local-development-setup)
- [Cosmos DB Configuration](#cosmos-db-configuration)
- [Running the Application](#running-the-application)
- [Troubleshooting](#troubleshooting)

## Prerequisites

Before you begin, ensure you have the following installed on your development machine:

- [Docker](https://www.docker.com/get-started) (version 20.10 or later)
- [Docker Compose](https://docs.docker.com/compose/install/) (version 1.29 or later)
- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)

## Local Development Setup

### 1. Clone the Repository

```bash
git clone https://github.com/davidpizon/Vera.git
cd Vera
```

### 2. Set Up Environment Variables

Copy the example environment file and customize if needed:

```bash
cp .env.example .env
```

The default values in `.env.example` are pre-configured for the local Cosmos DB Emulator.

### 3. Start the Local Cosmos DB Emulator

Use Docker Compose to start the Azure Cosmos DB Emulator:

```bash
docker compose up -d
```

**Note**: Depending on your Docker installation, you may need to use `docker-compose` (with hyphen) instead of `docker compose` (with space). Both commands work with different versions of Docker Compose.

This command will:
- Download the Azure Cosmos DB Emulator Docker image (if not already present)
- Start the emulator container in detached mode
- Expose the necessary ports for local development

To check if the emulator is running:

```bash
docker compose ps
```

To view logs:

```bash
docker compose logs -f cosmosdb
```

### 4. Access the Cosmos DB Emulator

Once running, you can access the Cosmos DB Emulator:

- **Web UI (Data Explorer)**: https://localhost:8081/_explorer/index.html
- **Primary Endpoint**: https://localhost:8081

**Note**: The emulator uses a self-signed SSL certificate. You may need to accept the security warning in your browser.

### 5. Import the Emulator SSL Certificate (Optional)

For production-like SSL validation, you may want to import the emulator's SSL certificate:

#### On Windows:
```powershell
# Download and install the certificate
curl -k https://localhost:8081/_explorer/emulator.pem > emulatorcert.crt
certutil -addstore -user -f Root emulatorcert.crt
```

#### On macOS:
```bash
# Download the certificate
curl -k https://localhost:8081/_explorer/emulator.pem > emulatorcert.crt
# Import to keychain
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain emulatorcert.crt
```

#### On Linux:
```bash
# Download the certificate
curl -k https://localhost:8081/_explorer/emulator.pem > /usr/local/share/ca-certificates/cosmosdb-emulator.crt
# Update certificates
sudo update-ca-certificates
```

## Cosmos DB Configuration

### Default Emulator Settings

The local development configuration uses the following default settings:

| Setting | Value | Description |
|---------|-------|-------------|
| Endpoint | `https://localhost:8081` | Local emulator endpoint |
| Primary Key | `C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==` | Default emulator key (well-known) |
| Database Name | `VeraDb` | Database for the application |
| Container Name | `Users` | Primary container for user data |
| SSL Validation | `false` | Disabled for local development |

**⚠️ Security Warning**: The default emulator key is publicly known and should **NEVER** be used in production environments. Always use a secure, unique key for production Azure Cosmos DB instances.

### Configuration Files

#### appsettings.Development.json

This file contains the Cosmos DB configuration for local development:

```json
{
  "CosmosDb": {
    "Endpoint": "https://localhost:8081",
    "Key": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "DatabaseName": "VeraDb",
    "ContainerName": "Users",
    "EnableSSLValidation": false
  }
}
```

#### docker-compose.yml

The Docker Compose file defines the local Cosmos DB Emulator service with:
- **Partition Count**: 10 (adjustable for your needs)
- **Data Persistence**: Enabled to retain data between restarts
- **Port Mappings**: All necessary ports exposed for local access

### Customizing Configuration

To customize the Cosmos DB configuration:

1. Edit `appsettings.Development.json` for application-specific settings
2. Edit `docker-compose.yml` for emulator-specific settings
3. Edit `.env` for environment-specific variables

## Running the Application

### Start the Cosmos DB Emulator

```bash
docker compose up -d
```

### Run Your Application

```bash
dotnet run
```

Or, if you're using a specific project file:

```bash
dotnet run --project ./src/Vera.Api/Vera.Api.csproj
```

### Stop the Emulator

When you're done developing:

```bash
docker compose down
```

To stop and remove all data:

```bash
docker compose down -v
```

## Troubleshooting

### Common Issues and Solutions

#### 1. Container Fails to Start

**Problem**: Docker container exits immediately or fails to start.

**Solutions**:
- Ensure Docker has enough resources allocated (at least 4GB RAM)
- Check Docker logs: `docker compose logs cosmosdb`
- Verify no other service is using ports 8081, 10251-10254
- Try pulling the latest image: `docker compose pull`

#### 2. Cannot Connect to Emulator

**Problem**: Application cannot connect to `https://localhost:8081`

**Solutions**:
- Verify the emulator is running: `docker compose ps`
- Check if ports are accessible: `curl -k https://localhost:8081`
- Ensure `EnableSSLValidation` is set to `false` in configuration
- Try restarting the container: `docker compose restart cosmosdb`

#### 3. SSL Certificate Errors

**Problem**: SSL/TLS errors when connecting to the emulator

**Solutions**:
- Set `EnableSSLValidation` to `false` in `appsettings.Development.json`
- Import the emulator SSL certificate (see instructions above)
- Use `HttpClientHandler` with certificate validation disabled in your code

#### 4. Data Not Persisting

**Problem**: Data disappears when container restarts

**Solutions**:
- Ensure `AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=true` in `docker-compose.yml`
- Verify the volume is created: `docker volume ls`
- Check volume mount is correct: `docker compose config`

#### 5. Performance Issues

**Problem**: Emulator is slow or unresponsive

**Solutions**:
- Increase Docker memory allocation (recommended: 4GB minimum)
- Reduce partition count in `docker-compose.yml` (if you don't need 10 partitions)
- Restart the emulator: `docker compose restart cosmosdb`

#### 6. Port Already in Use

**Problem**: Error binding to port 8081 or 10251-10254

**Solutions**:
- Identify the process using the port:
  - Windows: `netstat -ano | findstr :8081`
  - macOS/Linux: `lsof -i :8081`
- Stop the conflicting process or modify ports in `docker-compose.yml`

### Emulator Limitations

The Azure Cosmos DB Emulator has some limitations compared to the cloud service:

- **Performance**: Not representative of production performance
- **Storage**: Limited to available disk space
- **Features**: Some preview features may not be available
- **Scale**: Cannot test global distribution or multi-region scenarios
- **Platform**: Linux emulator is in preview; use for development only

### Getting Help

If you encounter issues not covered here:

1. Check the [Azure Cosmos DB Emulator documentation](https://learn.microsoft.com/azure/cosmos-db/local-emulator)
2. Review the [Docker Compose documentation](https://docs.docker.com/compose/)
3. Search for similar issues in the project's [GitHub Issues](https://github.com/davidpizon/Vera/issues)
4. Open a new issue with detailed information about your problem

---

## 📚 Additional Documentation

### Aspire Orchestration
- [ASPIRE_CONFIG_SUMMARY.md](ASPIRE_CONFIG_SUMMARY.md) - Quick overview of Aspire configuration
- [docs/ASPIRE_QUICKSTART.md](docs/ASPIRE_QUICKSTART.md) - 2-minute quick start guide
- [docs/ASPIRE_COORDINATION.md](docs/ASPIRE_COORDINATION.md) - Comprehensive coordination guide

### General Documentation
- [ASPIRE_SETUP.md](ASPIRE_SETUP.md) - Complete .NET Aspire setup guide
- [AZURE_DEPLOYMENT.md](AZURE_DEPLOYMENT.md) - Azure deployment with Aspire
- [src/Vera.BlazorHybrid/ASPIRE_INTEGRATION.md](src/Vera.BlazorHybrid/ASPIRE_INTEGRATION.md) - Mobile app integration guide
