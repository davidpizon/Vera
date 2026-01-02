using Vera.Application.DTOs;
using Vera.Domain.Entities;
using Vera.Domain.Interfaces;

namespace Vera.Application.Services;

public class MatchingApplicationService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMatchingService _matchingService;

    public MatchingApplicationService(
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IMatchingService matchingService)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _matchingService = matchingService;
    }

    public async Task GenerateMatchesAsync(string userId, CancellationToken cancellationToken = default)
    {
        await _matchingService.GenerateMatchesForUserAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<MatchDto>> GetMatchesForUserAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var matches = await _matchRepository.GetMatchesForUserAsync(userId, cancellationToken);
        var matchDtos = new List<MatchDto>();

        foreach (var match in matches)
        {
            var otherUserId = match.User1Id == userId ? match.User2Id : match.User1Id;
            var otherUser = await _userRepository.GetByIdAsync(otherUserId, cancellationToken);

            if (otherUser != null)
            {
                matchDtos.Add(new MatchDto
                {
                    Id = match.Id,
                    MatchedUser = MapToUserDto(otherUser),
                    CompatibilityScore = match.CompatibilityScore,
                    CreatedAt = match.CreatedAt,
                    IsMutualMatch = match.IsMutualMatch,
                    ScoreBreakdown = match.ScoreBreakdown
                });
            }
        }

        return matchDtos.OrderByDescending(m => m.CompatibilityScore);
    }

    public async Task<bool> ExpressInterestAsync(
        string userId, 
        string matchId, 
        CancellationToken cancellationToken = default)
    {
        var match = await _matchRepository.GetByIdAsync(matchId, cancellationToken);
        if (match == null) return false;

        if (match.User1Id == userId)
        {
            match.IsUser1Interested = true;
        }
        else if (match.User2Id == userId)
        {
            match.IsUser2Interested = true;
        }
        else
        {
            return false;
        }

        // Check if mutual match
        if (match.IsUser1Interested && match.IsUser2Interested)
        {
            match.IsMutualMatch = true;
            match.MatchedAt = DateTime.UtcNow;
        }

        await _matchRepository.UpdateAsync(match, cancellationToken);
        return match.IsMutualMatch;
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            ProfileImageUrl = user.ProfileImageUrl,
            Profile = user.Profile != null ? new UserProfileDto
            {
                Id = user.Profile.Id,
                Age = user.Profile.Age,
                Gender = user.Profile.Gender,
                Location = user.Profile.Location,
                Bio = user.Profile.Bio,
                Occupation = user.Profile.Occupation,
                Education = user.Profile.Education,
                Interests = user.Profile.Interests,
                Hobbies = user.Profile.Hobbies,
                IsComplete = user.Profile.IsComplete
            } : null
        };
    }
}
