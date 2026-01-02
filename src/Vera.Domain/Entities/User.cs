namespace Vera.Domain.Entities;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ExternalId { get; set; } = string.Empty; // From Entra External ID
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public UserProfile? Profile { get; set; }
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    public ICollection<Match> MatchesAsUser1 { get; set; } = new List<Match>();
    public ICollection<Match> MatchesAsUser2 { get; set; } = new List<Match>();
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}
