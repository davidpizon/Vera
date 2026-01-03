# Vera Blazor Hybrid - Aspire Integration

This guide explains how to configure the Vera Blazor Hybrid mobile app to connect to the backend API hosted by .NET Aspire.

## Configuration

The mobile app connects to the API using settings in `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5001",
    "UseServiceDiscovery": false
  }
}
```

### Development Setup

1. **Start the Aspire AppHost first**:
   ```bash
   cd src/Vera.AppHost
   dotnet run
   ```

2. **Note the API URL** from the Aspire dashboard (usually `https://localhost:5001`)

3. **Update the API BaseUrl** in `appsettings.Development.json` if needed

4. **Run the mobile app** using your preferred method (Visual Studio, VS Code, CLI)

### Platform-Specific Considerations

#### Android Emulator
- `localhost` refers to the emulator itself, not your development machine
- Use `10.0.2.2` instead of `localhost` for Android emulator:
  ```json
  "BaseUrl": "https://10.0.2.2:5001"
  ```
- You may need to handle SSL certificate validation for development

#### iOS Simulator
- `localhost` works correctly for iOS Simulator
- Use `https://localhost:5001` as configured

#### Physical Devices
- Use your development machine's IP address:
  ```json
  "BaseUrl": "https://192.168.1.100:5001"
  ```
- Ensure the device is on the same network
- You'll need to configure SSL certificates properly

## HttpClient Configuration

The `ApiService` uses `IHttpClientFactory` to create HTTP clients. Ensure your app startup code configures the named client:

```csharp
builder.Services.AddHttpClient("VeraAPI", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
```

### With Aspire Service Discovery (Future Enhancement)

For production scenarios, you can enable service discovery:

1. Update `Vera.BlazorHybrid.csproj` to include Aspire packages:
   ```xml
   <PackageReference Include="Microsoft.Extensions.ServiceDiscovery" Version="10.0.0" />
   ```

2. Enable service discovery in configuration:
   ```json
   "ApiSettings": {
     "UseServiceDiscovery": true,
     "ServiceName": "vera-api"
   }
   ```

3. Configure the HttpClient to use service discovery:
   ```csharp
   builder.Services.AddHttpClient("VeraAPI")
       .AddServiceDiscovery();
   ```

## SSL Development Certificates

### Trust Development Certificate (Windows/macOS)

```bash
dotnet dev-certs https --trust
```

### Android SSL Certificate Handling

For Android development, you may need to configure network security:

Create `Platforms/Android/Resources/xml/network_security_config.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <debug-overrides>
        <trust-anchors>
            <certificates src="system" />
            <certificates src="user" />
        </trust-anchors>
    </debug-overrides>
</network-security-config>
```

Reference it in `AndroidManifest.xml`:

```xml
<application android:networkSecurityConfig="@xml/network_security_config">
```

## Testing the Connection

1. Start the Aspire AppHost
2. Verify the API is running by accessing `https://localhost:5001/health` in a browser
3. Run the mobile app
4. Check the mobile app logs for connection errors

## Troubleshooting

### Connection Refused
- Ensure the Aspire AppHost is running
- Check the API URL in the Aspire dashboard
- Verify firewall settings allow connections

### SSL Certificate Errors
- Trust the development certificate
- For Android, configure network security config
- For production, use valid SSL certificates

### Authentication Issues
- Verify Azure AD configuration matches between API and mobile app
- Check token acquisition logs
- Ensure scopes are correctly configured

## Production Deployment

For production:

1. Deploy the API to Azure (App Service, Container Apps, etc.)
2. Update `appsettings.json` with the production API URL
3. Use Azure Front Door or API Management if needed
4. Ensure proper SSL certificates are configured
5. Consider using Azure App Configuration for dynamic settings
