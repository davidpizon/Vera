namespace Vera.Application.DTOs;

public class MatchDto
{
    public string Id { get; set; } = string.Empty;
    public UserDto? MatchedUser { get; set; }
    public double CompatibilityScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsMutualMatch { get; set; }
    public Dictionary<string, double> ScoreBreakdown { get; set; } = new();
}

public class PhotoDto
{
    public string Id { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? AiFeedback { get; set; }
    public double? AiQualityScore { get; set; }
}

public class PhotoUploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public bool IsPrimary { get; set; }
}

public class PhotoUploadResponse
{
    public string PhotoId { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public string? AiFeedback { get; set; }
    public double? AiQualityScore { get; set; }
}
