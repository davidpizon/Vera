using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Vera.Application.DTOs;
using Vera.Application.Services;

namespace Vera.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope("access_as_user")]
public class ConversationController : ControllerBase
{
    private readonly ConversationService _conversationService;

    public ConversationController(ConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("oid")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        var response = await _conversationService.ProcessMessageAsync(userId, request, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConversations(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("oid")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        var conversations = await _conversationService.GetUserConversationsAsync(userId, cancellationToken);
        return Ok(conversations);
    }
}
