using Microsoft.Azure.Cosmos;
using Vera.Domain.Interfaces;
using DomainUser = Vera.Domain.Entities.User;

namespace Vera.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CosmosDbContext _context;

    public UserRepository(CosmosDbContext context)
    {
        _context = context;
    }

    public async Task<DomainUser?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _context.Users.ReadItemAsync<DomainUser>(id, new PartitionKey(id), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<DomainUser?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.externalId = @externalId")
            .WithParameter("@externalId", externalId);

        var iterator = _context.Users.GetItemQueryIterator<DomainUser>(query);
        var results = await iterator.ReadNextAsync(cancellationToken);
        return results.FirstOrDefault();
    }

    public async Task<DomainUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.email = @email")
            .WithParameter("@email", email);

        var iterator = _context.Users.GetItemQueryIterator<DomainUser>(query);
        var results = await iterator.ReadNextAsync(cancellationToken);
        return results.FirstOrDefault();
    }

    public async Task<DomainUser> CreateAsync(DomainUser user, CancellationToken cancellationToken = default)
    {
        var response = await _context.Users.CreateItemAsync(user, new PartitionKey(user.Id), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<DomainUser> UpdateAsync(DomainUser user, CancellationToken cancellationToken = default)
    {
        var response = await _context.Users.ReplaceItemAsync(user, user.Id, new PartitionKey(user.Id), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _context.Users.DeleteItemAsync<DomainUser>(id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}
