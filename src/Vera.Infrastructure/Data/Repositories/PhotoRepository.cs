using Microsoft.Azure.Cosmos;
using Vera.Domain.Entities;
using Vera.Domain.Interfaces;

namespace Vera.Infrastructure.Data.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly CosmosDbContext _context;

    public PhotoRepository(CosmosDbContext context)
    {
        _context = context;
    }

    public async Task<Photo?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // Since Photos are partitioned by userId, we need to query across partitions
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
            .WithParameter("@id", id);

        var iterator = _context.Photos.GetItemQueryIterator<Photo>(query);
        
        if (iterator.HasMoreResults)
        {
            var results = await iterator.ReadNextAsync(cancellationToken);
            return results.FirstOrDefault();
        }
        
        return null;
    }

    public async Task<IEnumerable<Photo>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId")
            .WithParameter("@userId", userId);

        var iterator = _context.Photos.GetItemQueryIterator<Photo>(query);
        var results = new List<Photo>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response);
        }

        return results;
    }

    public async Task<Photo> CreateAsync(Photo photo, CancellationToken cancellationToken = default)
    {
        var response = await _context.Photos.CreateItemAsync(photo, new PartitionKey(photo.UserId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<Photo> UpdateAsync(Photo photo, CancellationToken cancellationToken = default)
    {
        var response = await _context.Photos.ReplaceItemAsync(photo, photo.Id, new PartitionKey(photo.UserId), cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        // First get the photo to retrieve its UserId (partition key)
        var photo = await GetByIdAsync(id, cancellationToken);
        if (photo != null)
        {
            await _context.Photos.DeleteItemAsync<Photo>(id, new PartitionKey(photo.UserId), cancellationToken: cancellationToken);
        }
    }
}
