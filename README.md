# Vera - AI-Powered Dating Application

Vera is a modern, cross-platform dating application built with .NET 10, featuring AI-powered conversational profile creation, intelligent matching algorithms, and secure cloud infrastructure.

## 🌟 Features

- **AI Conversational Assistant**: Natural language profile creation using Azure OpenAI
- **Smart Matching Algorithm**: Advanced compatibility scoring based on conversation data
- **Photo Upload with AI Feedback**: Intelligent photo analysis and recommendations
- **Cross-Platform Mobile App**: Native iOS and Android support via .NET MAUI Blazor Hybrid
- **Secure Authentication**: Microsoft Entra External ID with Google/Facebook login
- **Cloud-Native Architecture**: Azure Cosmos DB for global scale and performance
- **Data Encryption**: End-to-end encryption for sensitive user data
- **Docker Support**: Containerized backend for easy deployment

## 🏗️ Architecture

The application follows Clean Architecture principles with clear separation of concerns:

```
Vera/
├── src/
│   ├── Vera.Domain/          # Domain entities and interfaces
│   ├── Vera.Application/     # Business logic and use cases
│   ├── Vera.Infrastructure/  # External services (Cosmos DB, Azure OpenAI)
│   ├── Vera.API/            # REST API backend (.NET 10)
│   └── Vera.BlazorHybrid/   # Mobile app (iOS & Android)
├── .github/
│   ├── workflows/           # CI/CD pipelines
│   └── dependabot.yml      # Dependency scanning
├── Dockerfile              # API containerization
└── docker-compose.yml      # Local development setup
```

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- Azure Subscription (for Cosmos DB, OpenAI, and Entra ID)
- Visual Studio 2022 or VS Code
- Docker Desktop (optional, for containerization)

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

Update the API endpoint in `src/Vera.BlazorHybrid/MauiProgram.cs`:

```csharp
builder.Services.AddHttpClient("VeraAPI", client =>
{
    client.BaseAddress = new Uri("https://your-api-url.azurewebsites.net/");
});
```

Update authentication settings in `src/Vera.BlazorHybrid/Services/AuthenticationService.cs`:

```csharp
_msalClient = PublicClientApplicationBuilder
    .Create("YOUR_CLIENT_ID")
    .WithAuthority("https://login.microsoftonline.com/YOUR_TENANT_ID")
    .Build();
```

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

## 🛠️ Technology Stack

### Backend
- .NET 10.0
- ASP.NET Core Web API
- Azure Cosmos DB
- Azure OpenAI (GPT-4)
- Microsoft Identity Web

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

