# Vera.BlazorHybrid - Mobile Application

This project contains the Blazor Hybrid mobile application for Vera, targeting iOS and Android platforms via .NET MAUI.

## Important Notes

### Current Status
This project is currently configured as a Blazor component library (TargetFramework: net10.0) for demonstration purposes. To build actual iOS and Android applications, you need to:

1. **Install .NET MAUI Workload** on your development machine:
   ```bash
   dotnet workload install maui
   ```

2. **Update the csproj file** to use multi-targeting:
   ```xml
   <TargetFrameworks>net10.0-android;net10.0-ios</TargetFrameworks>
   <UseMaui>true</UseMaui>
   ```

3. **Restore MAUI packages**:
   ```bash
   dotnet restore
   ```

### Building for Mobile Platforms

#### Android
```bash
dotnet build -t:Run -f net10.0-android
```

Requirements:
- Android SDK
- Android Emulator or physical device

#### iOS (macOS only)
```bash
dotnet build -t:Run -f net10.0-ios
```

Requirements:
- macOS with Xcode installed
- iOS Simulator or physical device

### Platform-Specific Files

The following platform-specific files are included and will be compiled when MAUI workload is installed:
- `Platforms/Android/MainActivity.cs`
- `Platforms/Android/MainApplication.cs`
- `Platforms/Android/AndroidManifest.xml`
- `Platforms/iOS/AppDelegate.cs`
- `Platforms/iOS/Program.cs`
- `Platforms/iOS/Info.plist`

## Features

- **Cross-Platform UI**: Blazor Hybrid provides native performance with web-based UI
- **Azure Authentication**: Microsoft Entra External ID integration with MSAL
- **API Integration**: Communicates with Vera.API backend
- **Offline Support**: Can be extended with local storage capabilities

## Configuration

Update the API endpoint in `Services/AuthenticationService.cs` and `MauiProgram.cs` with your deployed API URL.

For detailed setup instructions, see the main README.md in the repository root.
