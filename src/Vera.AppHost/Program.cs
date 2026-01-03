namespace Vera.AppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        // Add Azure Cosmos DB emulator or connection
        var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
            .RunAsEmulator(emulator => emulator
                .WithLifetime(ContainerLifetime.Persistent));

        var database = cosmosDb.AddDatabase("VeraDb");

        // Add Azure OpenAI (using configuration from user secrets or appsettings)
        var openai = builder.AddConnectionString("azureopenai");

        // Add the API with service discovery
        var api = builder.AddProject<Projects.Vera_API>("vera-api")
            .WithReference(database)
            .WithEnvironment("AzureOpenAI__ConnectionString", openai)
            .WithExternalHttpEndpoints();

        // For mobile/hybrid development, the BlazorHybrid app will connect to the API
        // The API endpoint will be available for the mobile app to reference
        api.WithEnvironment("ASPNETCORE_URLS", "http://localhost:5000;https://localhost:5001");

        builder.Build().Run();
    }
}
