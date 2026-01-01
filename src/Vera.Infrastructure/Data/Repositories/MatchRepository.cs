using Microsoft.Azure.Cosmos;
using Vera.Domain.Entities;
using Vera.Domain.Interfaces;

namespace Vera.Infrastructure.Data.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly CosmosDbContext _context;

    public MatchRepository(CosmosDbContext context)
    {
        _context = context;
    }

    public async Task<Match?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _context.Matches.ReadItemAsync<Match>(id, new PartitionKey(id), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Match>> GetMatchesForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.user1Id = @userId OR c.user2Id = @userId")
            .WithParameter("@userId", userId);

        var iterator = _context.Matches.GetItemQueryIterator<Match>(query);
        var results = new List<Match>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }

    public async Task<IEnumerable<Match>> GetMutualMatchesForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE (c.user1Id = @userId OR c.user2Id = @userId) AND c.isMutualMatch = true")
            .WithParameter("@userId", userId);

        var iterator = _context.Matches.GetItemQueryIterator<Match>(query);
        var results = new List<Match>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }

    public async Task<Match> CreateAsync(Match match, CancellationToken cancellationToken = default)
    {
        var response = await _context.Matches.CreateItemAsync(match, new PartitionKey(match.Id), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<Match> UpdateAsync(Match match, CancellationToken cancellationToken = default)
    {
        var response = await _context.Matches.ReplaceItemAsync(match, match.Id, new PartitionKey(match.Id), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _context.Matches.DeleteItemAsync<Match>(id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}
