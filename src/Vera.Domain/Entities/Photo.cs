namespace Vera.Domain.Entities;

public class Photo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? AiFeedback { get; set; }
    public double? AiQualityScore { get; set; }
    public bool IsApproved { get; set; } = false;
    
    // Navigation properties
    public User? User { get; set; }
}
