using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ImageController : ControllerBase{

  private readonly PurpuraDbContext _dbContext;

  public ImageController(PurpuraDbContext dbContext){
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
  }

 [HttpPost("upload")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
    {
       try
       {
         if (image == null || image.Length == 0)
        {
            return BadRequest(new { success = false, message = "No image provided." });
        }
        using var stream = image.OpenReadStream();
        ImageService imageService = new ();
        string imageId = Guid.NewGuid().ToString();
        var imageUrl = await imageService.UploadImageAsync(stream, imageId);

        if (string.IsNullOrEmpty(imageUrl))
        {
            return StatusCode(500, new { success = false, message = "Image upload failed." });
        }

        return Ok(new { success = true, message = "Image uploaded successfully.", url = imageUrl });
       }
       catch (System.Exception)
       {
        
        return StatusCode(500, new { success = false, message = "An unexpected error occured." });
       }
    }




}