using Microsoft.Azure.Cosmos;
using Vera.Domain.Entities;
using Vera.Domain.Interfaces;

namespace Vera.Infrastructure.Data.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly CosmosDbContext _context;

    public ConversationRepository(CosmosDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _context.Conversations.ReadItemAsync<Conversation>(id, new PartitionKey(id), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Conversation>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId")
            .WithParameter("@userId", userId);

        var iterator = _context.Conversations.GetItemQueryIterator<Conversation>(query);
        var results = new List<Conversation>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }

    public async Task<Conversation> CreateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        var response = await _context.Conversations.CreateItemAsync(conversation, new PartitionKey(conversation.UserId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<Conversation> UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        var response = await _context.Conversations.ReplaceItemAsync(conversation, conversation.Id, new PartitionKey(conversation.UserId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _context.Conversations.DeleteItemAsync<Conversation>(id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}
