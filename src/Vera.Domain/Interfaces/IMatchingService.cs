namespace Vera.Domain.Interfaces;

public interface IMatchingService
{
    Task<double> CalculateCompatibilityScoreAsync(string userId1, string userId2, CancellationToken cancellationToken = default);
    Task GenerateMatchesForUserAsync(string userId, CancellationToken cancellationToken = default);
}
