namespace Vera.Domain.Entities;

public class Conversation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string ConversationType { get; set; } = string.Empty; // e.g., "ProfileCreation", "PreferencesGathering"
    public bool IsComplete { get; set; } = false;
    public List<ConversationMessage> Messages { get; set; } = new();
    
    // Navigation properties
    public User? User { get; set; }
}

public class ConversationMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
