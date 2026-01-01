# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Vera.sln", "./"]
COPY ["src/Vera.Domain/Vera.Domain.csproj", "src/Vera.Domain/"]
COPY ["src/Vera.Application/Vera.Application.csproj", "src/Vera.Application/"]
COPY ["src/Vera.Infrastructure/Vera.Infrastructure.csproj", "src/Vera.Infrastructure/"]
COPY ["src/Vera.API/Vera.API.csproj", "src/Vera.API/"]

# Restore dependencies
RUN dotnet restore "src/Vera.API/Vera.API.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/Vera.API"
RUN dotnet build "Vera.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Vera.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run as non-root user for security
USER $APP_UID

ENTRYPOINT ["dotnet", "Vera.API.dll"]
