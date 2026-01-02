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

## Additional Resources

- [Azure Cosmos DB Documentation](https://learn.microsoft.com/azure/cosmos-db/)
- [Azure Cosmos DB Emulator](https://learn.microsoft.com/azure/cosmos-db/local-emulator)
- [Docker Documentation](https://docs.docker.com/)
- [.NET SDK for Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/nosql/sdk-dotnet-v3)
