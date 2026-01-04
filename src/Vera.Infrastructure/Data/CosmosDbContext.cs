using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace Vera.Infrastructure.Data;

public class CosmosDbContext
{
    private readonly CosmosClient _client;
    private readonly Database _database;
    private readonly string _databaseName;

    public CosmosDbContext(IConfiguration configuration)
    {
        // Support both Aspire connection string and traditional configuration
        var connectionString = configuration.GetConnectionString("cosmosdb");
        
        if (!string.IsNullOrEmpty(connectionString))
        {
            // Aspire-injected connection string
            _client = new CosmosClient(connectionString);
            _databaseName = configuration["CosmosDb:DatabaseName"] ?? "VeraDb";
        }
        else
        {
            // Traditional configuration
            var endpoint = configuration["CosmosDb:Endpoint"] ?? throw new InvalidOperationException("CosmosDb:Endpoint not configured");
            var key = configuration["CosmosDb:Key"] ?? throw new InvalidOperationException("CosmosDb:Key not configured");
            _databaseName = configuration["CosmosDb:DatabaseName"] ?? "VeraDb";
            _client = new CosmosClient(endpoint, key);
        }
        
        _database = _client.GetDatabase(_databaseName);
    }

    public Container Users => _database.GetContainer("Users");
    public Container Conversations => _database.GetContainer("Conversations");
    public Container Matches => _database.GetContainer("Matches");
    public Container Photos => _database.GetContainer("Photos");

    public async Task InitializeDatabaseAsync()
    {
        // Create database if it doesn't exist
        await _client.CreateDatabaseIfNotExistsAsync(_databaseName);

        // Create containers
        await _database.CreateContainerIfNotExistsAsync("Users", "/id");
        await _database.CreateContainerIfNotExistsAsync("Conversations", "/userId");
        await _database.CreateContainerIfNotExistsAsync("Matches", "/id");
        await _database.CreateContainerIfNotExistsAsync("Photos", "/userId");
    }
}
