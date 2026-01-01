using Vera.Application.DTOs;
using Vera.Domain.Entities;
using Vera.Domain.Interfaces;

namespace Vera.Application.Services;

public class ConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAIConversationService _aiService;

    public ConversationService(
        IConversationRepository conversationRepository,
        IUserRepository userRepository,
        IAIConversationService aiService)
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
        _aiService = aiService;
    }

    public async Task<ChatResponse> ProcessMessageAsync(
        string userId, 
        ChatRequest request, 
        CancellationToken cancellationToken = default)
    {
        // Get or create conversation
        var conversations = await _conversationRepository.GetByUserIdAsync(userId, cancellationToken);
        var conversation = conversations
            .Where(c => c.ConversationType == request.ConversationType && !c.IsComplete)
            .OrderByDescending(c => c.StartedAt)
            .FirstOrDefault();

        if (conversation == null)
        {
            conversation = new Conversation
            {
                UserId = userId,
                ConversationType = request.ConversationType,
                StartedAt = DateTime.UtcNow
            };
            conversation = await _conversationRepository.CreateAsync(conversation, cancellationToken);
        }

        // Add user message
        conversation.Messages.Add(new ConversationMessage
        {
            Role = "user",
            Content = request.Message,
            Timestamp = DateTime.UtcNow
        });

        // Get AI response
        var aiResponse = await _aiService.GetResponseAsync(
            userId, 
            request.Message, 
            request.ConversationType, 
            cancellationToken);

        // Add assistant message
        conversation.Messages.Add(new ConversationMessage
        {
            Role = "assistant",
            Content = aiResponse,
            Timestamp = DateTime.UtcNow
        });

        // Update conversation
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);

        // Update user profile if conversation is complete
        if (conversation.IsComplete)
        {
            await UpdateUserProfileFromConversationAsync(userId, conversation, cancellationToken);
        }

        return new ChatResponse
        {
            Message = aiResponse,
            ConversationId = conversation.Id,
            IsComplete = conversation.IsComplete
        };
    }

    private async Task UpdateUserProfileFromConversationAsync(
        string userId, 
        Conversation conversation, 
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return;

        // Extract profile information from conversation messages
        // This is a simplified version - in production, use more sophisticated NLP
        var userMessages = conversation.Messages.Where(m => m.Role == "user").ToList();
        
        if (user.Profile != null)
        {
            user.Profile.IsComplete = true;
            user.Profile.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }

    public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var conversations = await _conversationRepository.GetByUserIdAsync(userId, cancellationToken);
        
        return conversations.Select(c => new ConversationDto
        {
            Id = c.Id,
            UserId = c.UserId,
            StartedAt = c.StartedAt,
            CompletedAt = c.CompletedAt,
            ConversationType = c.ConversationType,
            IsComplete = c.IsComplete,
            Messages = c.Messages.Select(m => new MessageDto
            {
                Role = m.Role,
                Content = m.Content,
                Timestamp = m.Timestamp
            }).ToList()
        });
    }
}
