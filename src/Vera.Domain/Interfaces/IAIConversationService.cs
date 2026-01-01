namespace Vera.Domain.Interfaces;

public interface IAIConversationService
{
    Task<string> GetResponseAsync(string userId, string userMessage, string conversationType, CancellationToken cancellationToken = default);
    Task<string> GetPhotoFeedbackAsync(Stream photoStream, CancellationToken cancellationToken = default);
}
