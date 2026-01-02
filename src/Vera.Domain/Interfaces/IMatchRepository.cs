using Vera.Domain.Entities;

namespace Vera.Domain.Interfaces;

public interface IMatchRepository
{
    Task<Match?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetMatchesForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Match>> GetMutualMatchesForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Match> CreateAsync(Match match, CancellationToken cancellationToken = default);
    Task<Match> UpdateAsync(Match match, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
