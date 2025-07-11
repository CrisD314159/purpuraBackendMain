using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Controllers;


[ApiController]
[Route("[controller]")]
public class MediaUploadController(IMediaUploadService mediaUploadService) : ControllerBase
{
    private readonly IMediaUploadService _mediaUploadService = mediaUploadService;

  [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost("image")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        var url = await _mediaUploadService.UploadImageAsync(file);
        return Ok(new { Url = url });
    }

  [Authorize(Roles ="ADMIN", AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost("audio")]
    public async Task<IActionResult> UploadAudio([FromForm] IFormFile file)
    {
        var url = await _mediaUploadService.UploadAudioAsync(file);
        return Ok(new { Url = url });
    }
}