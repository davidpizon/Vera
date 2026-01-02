namespace Vera.Domain.Entities;

public class Match
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string User1Id { get; set; } = string.Empty;
    public string User2Id { get; set; } = string.Empty;
    public double CompatibilityScore { get; set; } = 0.0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsUser1Interested { get; set; } = false;
    public bool IsUser2Interested { get; set; } = false;
    public bool IsMutualMatch { get; set; } = false;
    public DateTime? MatchedAt { get; set; }
    public Dictionary<string, double> ScoreBreakdown { get; set; } = new();
    
    // Navigation properties
    public User? User1 { get; set; }
    public User? User2 { get; set; }
}
