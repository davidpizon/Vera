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
public class MatchController : ControllerBase
{
    private readonly MatchingApplicationService _matchingService;

    public MatchController(MatchingApplicationService matchingService)
    {
        _matchingService = matchingService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateMatches(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("oid")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        await _matchingService.GenerateMatchesAsync(userId, cancellationToken);
        return Ok(new { message = "Matches generated successfully" });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatches(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("oid")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        var matches = await _matchingService.GetMatchesForUserAsync(userId, cancellationToken);
        return Ok(matches);
    }

    [HttpPost("{matchId}/interest")]
    public async Task<ActionResult<object>> ExpressInterest(string matchId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("oid")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        var isMutualMatch = await _matchingService.ExpressInterestAsync(userId, matchId, cancellationToken);
        return Ok(new { isMutualMatch });
    }
}
