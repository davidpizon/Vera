using Vera.Domain.Entities;

namespace Vera.Domain.Interfaces;

public interface IPhotoRepository
{
    Task<Photo?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Photo>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Photo> CreateAsync(Photo photo, CancellationToken cancellationToken = default);
    Task<Photo> UpdateAsync(Photo photo, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
