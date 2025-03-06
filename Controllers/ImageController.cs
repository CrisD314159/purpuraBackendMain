using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

/// <summary>
/// Controlador para la gestión de imágenes.
/// </summary>
/// <remarks>
/// Este controlador permite subir imágenes a la aplicación.
/// </remarks>
[ApiController]
[Route("[controller]")]
[Authorize]
public class ImageController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    /// Constructor del controlador de imágenes.

    /// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
    public ImageController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }


    /// Sube una imagen y devuelve la URL de acceso. (No aparece en la documentación de la API de Swagger)
    /// <param name="image">Archivo de imagen a subir.</param>
    /// <returns>URL de la imagen subida.</returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
    {
        try
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new { success = false, message = "No image provided." });
            }

            using var stream = image.OpenReadStream();
            ImageService imageService = new();
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
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }
}