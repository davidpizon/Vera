namespace Vera.Application.DTOs;

public class ConversationDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string ConversationType { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public List<MessageDto> Messages { get; set; } = new();
}

public class MessageDto
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string ConversationType { get; set; } = "ProfileCreation";
}

public class ChatResponse
{
    public string Message { get; set; } = string.Empty;
    public string ConversationId { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}
