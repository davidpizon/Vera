namespace Vera.AppHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        // Aspire 13.x pattern: Azure Cosmos DB Emulator
        var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
            .RunAsEmulator(emulator =>
            {
                emulator.WithDataVolume("vera-cosmosdb-data");
            });

        var database = cosmosDb.AddCosmosDatabase("VeraDb");

        // Aspire 13.x pattern: Azure Storage Emulator (Azurite)
        var storage = builder.AddAzureStorage("storage")
            .RunAsEmulator();
        
        var tables = storage.AddTables("tables");

        // Connection string for Azure OpenAI
        var openai = builder.AddConnectionString("azureopenai");

        // Add the API project with service discovery
        var api = builder.AddProject("vera-api", "../Vera.API/Vera.API.csproj")
            .WithReference(database)
            .WithReference(tables)
            .WithEnvironment("AzureOpenAI__ConnectionString", openai)
            .WithExternalHttpEndpoints();

        builder.Build().Run();
    }
}
