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
public class PhotoController : ControllerBase
{
    private readonly PhotoService _photoService;

    public PhotoController(PhotoService photoService)
    {
        _photoService = photoService;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<PhotoUploadResponse>> UploadPhoto([FromBody] PhotoUploadRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("oid")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        var response = await _photoService.UploadPhotoAsync(userId, request, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotos(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("oid")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        var photos = await _photoService.GetUserPhotosAsync(userId, cancellationToken);
        return Ok(photos);
    }

    [HttpDelete("{photoId}")]
    public async Task<IActionResult> DeletePhoto(string photoId, CancellationToken cancellationToken)
    {
        await _photoService.DeletePhotoAsync(photoId, cancellationToken);
        return NoContent();
    }
}
