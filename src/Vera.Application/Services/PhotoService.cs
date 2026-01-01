using Vera.Application.DTOs;
using Vera.Domain.Entities;
using Vera.Domain.Interfaces;

namespace Vera.Application.Services;

public class PhotoService
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IAIConversationService _aiService;

    public PhotoService(IPhotoRepository photoRepository, IAIConversationService aiService)
    {
        _photoRepository = photoRepository;
        _aiService = aiService;
    }

    public async Task<PhotoUploadResponse> UploadPhotoAsync(
        string userId, 
        PhotoUploadRequest request, 
        CancellationToken cancellationToken = default)
    {
        // Get AI feedback on photo
        using var photoStream = new MemoryStream(request.Data);
        var aiFeedback = await _aiService.GetPhotoFeedbackAsync(photoStream, cancellationToken);

        // Create photo entity
        var photo = new Photo
        {
            UserId = userId,
            StorageUrl = $"photos/{userId}/{Guid.NewGuid()}.jpg", // Placeholder - would use actual blob storage
            ThumbnailUrl = $"photos/{userId}/thumb_{Guid.NewGuid()}.jpg",
            IsPrimary = request.IsPrimary,
            UploadedAt = DateTime.UtcNow,
            AiFeedback = aiFeedback,
            AiQualityScore = CalculateQualityScore(aiFeedback),
            IsApproved = true
        };

        var savedPhoto = await _photoRepository.CreateAsync(photo, cancellationToken);

        return new PhotoUploadResponse
        {
            PhotoId = savedPhoto.Id,
            StorageUrl = savedPhoto.StorageUrl,
            AiFeedback = savedPhoto.AiFeedback,
            AiQualityScore = savedPhoto.AiQualityScore
        };
    }

    public async Task<IEnumerable<PhotoDto>> GetUserPhotosAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var photos = await _photoRepository.GetByUserIdAsync(userId, cancellationToken);
        
        return photos.Select(p => new PhotoDto
        {
            Id = p.Id,
            StorageUrl = p.StorageUrl,
            ThumbnailUrl = p.ThumbnailUrl,
            IsPrimary = p.IsPrimary,
            DisplayOrder = p.DisplayOrder,
            UploadedAt = p.UploadedAt,
            AiFeedback = p.AiFeedback,
            AiQualityScore = p.AiQualityScore
        }).OrderBy(p => p.DisplayOrder);
    }

    public async Task DeletePhotoAsync(string photoId, CancellationToken cancellationToken = default)
    {
        await _photoRepository.DeleteAsync(photoId, cancellationToken);
    }

    private double CalculateQualityScore(string feedback)
    {
        // Simplified scoring - in production, use more sophisticated analysis
        var score = 0.5;
        
        if (feedback.Contains("good", StringComparison.OrdinalIgnoreCase) || 
            feedback.Contains("excellent", StringComparison.OrdinalIgnoreCase))
            score += 0.3;
            
        if (feedback.Contains("clear", StringComparison.OrdinalIgnoreCase))
            score += 0.2;
            
        return Math.Min(score, 1.0);
    }
}
