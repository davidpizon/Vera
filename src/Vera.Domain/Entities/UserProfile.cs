namespace Vera.Domain.Entities;

public class UserProfile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Occupation { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public List<string> Interests { get; set; } = new();
    public List<string> Hobbies { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsComplete { get; set; } = false;
    
    // Navigation properties
    public User? User { get; set; }
    public RelationshipPreferences? Preferences { get; set; }
}
