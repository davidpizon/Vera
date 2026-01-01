namespace Vera.Domain.Entities;

public class RelationshipPreferences
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserProfileId { get; set; } = string.Empty;
    public string PreferredGender { get; set; } = string.Empty;
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 99;
    public int MaxDistance { get; set; } = 50; // in kilometers
    public string RelationshipType { get; set; } = string.Empty; // e.g., "Long-term", "Casual", "Friendship"
    public List<string> DealBreakers { get; set; } = new();
    public List<string> MustHaves { get; set; } = new();
    public List<string> NiceToHaves { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public UserProfile? UserProfile { get; set; }
}
