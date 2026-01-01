using Vera.Domain.Entities;
using Vera.Domain.Interfaces;

namespace Vera.Infrastructure.Services;

public class MatchingService : IMatchingService
{
    private readonly IUserRepository _userRepository;
    private readonly IMatchRepository _matchRepository;

    public MatchingService(IUserRepository userRepository, IMatchRepository matchRepository)
    {
        _userRepository = userRepository;
        _matchRepository = matchRepository;
    }

    public async Task<double> CalculateCompatibilityScoreAsync(
        string userId1, 
        string userId2, 
        CancellationToken cancellationToken = default)
    {
        var user1 = await _userRepository.GetByIdAsync(userId1, cancellationToken);
        var user2 = await _userRepository.GetByIdAsync(userId2, cancellationToken);

        if (user1?.Profile == null || user2?.Profile == null)
            return 0.0;

        var scoreBreakdown = new Dictionary<string, double>();

        // Age compatibility (0-20 points)
        var ageScore = CalculateAgeCompatibility(user1.Profile, user2.Profile);
        scoreBreakdown["age"] = ageScore;

        // Shared interests (0-30 points)
        var interestsScore = CalculateSharedInterests(user1.Profile, user2.Profile);
        scoreBreakdown["interests"] = interestsScore;

        // Location proximity (0-20 points) - simplified
        var locationScore = CalculateLocationScore(user1.Profile, user2.Profile);
        scoreBreakdown["location"] = locationScore;

        // Education level compatibility (0-15 points)
        var educationScore = CalculateEducationCompatibility(user1.Profile, user2.Profile);
        scoreBreakdown["education"] = educationScore;

        // Preferences match (0-15 points)
        var preferencesScore = CalculatePreferencesMatch(user1.Profile, user2.Profile);
        scoreBreakdown["preferences"] = preferencesScore;

        var totalScore = scoreBreakdown.Values.Sum();
        return Math.Round(totalScore / 100.0, 2); // Normalize to 0-1
    }

    public async Task GenerateMatchesForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (currentUser?.Profile?.Preferences == null)
            return;

        // In a real implementation, this would query users based on preferences
        // For now, we'll create a simplified version
        
        // This is a placeholder - in production, implement proper user querying
        // based on preferences, location, age range, etc.
    }

    private double CalculateAgeCompatibility(UserProfile profile1, UserProfile profile2)
    {
        var ageDifference = Math.Abs(profile1.Age - profile2.Age);
        
        if (ageDifference <= 3) return 20.0;
        if (ageDifference <= 5) return 15.0;
        if (ageDifference <= 10) return 10.0;
        if (ageDifference <= 15) return 5.0;
        return 0.0;
    }

    private double CalculateSharedInterests(UserProfile profile1, UserProfile profile2)
    {
        var allInterests1 = profile1.Interests.Concat(profile1.Hobbies).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var allInterests2 = profile2.Interests.Concat(profile2.Hobbies).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var sharedCount = allInterests1.Intersect(allInterests2, StringComparer.OrdinalIgnoreCase).Count();
        var totalUnique = allInterests1.Union(allInterests2, StringComparer.OrdinalIgnoreCase).Count();

        if (totalUnique == 0) return 0.0;

        var jaccardScore = (double)sharedCount / totalUnique;
        return jaccardScore * 30.0;
    }

    private double CalculateLocationScore(UserProfile profile1, UserProfile profile2)
    {
        // Simplified - in production, calculate actual distance
        if (string.Equals(profile1.Location, profile2.Location, StringComparison.OrdinalIgnoreCase))
            return 20.0;
        
        return 10.0; // Different location but within range
    }

    private double CalculateEducationCompatibility(UserProfile profile1, UserProfile profile2)
    {
        if (string.Equals(profile1.Education, profile2.Education, StringComparison.OrdinalIgnoreCase))
            return 15.0;
        
        // Both have higher education
        var higherEd = new[] { "bachelor", "master", "phd", "doctorate" };
        var both = higherEd.Any(e => profile1.Education.Contains(e, StringComparison.OrdinalIgnoreCase)) &&
                   higherEd.Any(e => profile2.Education.Contains(e, StringComparison.OrdinalIgnoreCase));
        
        if (both) return 10.0;
        
        return 5.0;
    }

    private double CalculatePreferencesMatch(UserProfile profile1, UserProfile profile2)
    {
        var score = 0.0;

        // Check if age is within preferred range
        if (profile1.Preferences != null && profile2.Age >= profile1.Preferences.MinAge && profile2.Age <= profile1.Preferences.MaxAge)
            score += 7.5;

        if (profile2.Preferences != null && profile1.Age >= profile2.Preferences.MinAge && profile1.Age <= profile2.Preferences.MaxAge)
            score += 7.5;

        return score;
    }
}
