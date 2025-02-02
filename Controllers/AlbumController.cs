namespace purpuraMain.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

/// <summary>
/// Controlador para gestionar las operaciones relacionadas con los álbumes.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class AlbumController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    /// <summary>
    /// Constructor del controlador AlbumController.
    /// </summary>
    /// <param name="dbContext">Contexto de base de datos para acceder a la información de los álbumes.</param>
    public AlbumController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Obtiene un álbum por su identificador único.
    /// </summary>
    /// <param name="id">Identificador del álbum.</param>
    /// <returns>Un objeto GetAlbumDTO con los detalles del álbum.</returns>
    [HttpGet("getAlbum/{id}")]
    public async Task<ActionResult<GetAlbumDTO>> GetAlbumById(string id)
    {
        try
        {
            var album = await AlbumService.GetAlbumById(id, _dbContext);
            return Ok(album);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new { message = "Album Not found", success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }

    /// <summary>
    /// Busca álbumes basándose en un texto de entrada.
    /// </summary>
    /// <param name="input">Texto a buscar en los álbumes.</param>
    /// <param name="offset">Número de elementos a omitir en la paginación.</param>
    /// <param name="limit">Número máximo de álbumes a devolver.</param>
    /// <returns>Una lista de objetos GetAlbumDTO con los álbumes encontrados.</returns>
    [HttpGet("getAlbumByInput/{input}")]
    public async Task<ActionResult<List<GetAlbumDTO>>> GetAlbumByInput(string input, int offset, int limit)
    {
        try
        {
            var albums = await AlbumService.GetAlbumByInput(input, offset, limit, _dbContext);
            return Ok(albums);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new { message = "Album Not found", success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }

    /// <summary>
    /// Obtiene una lista de álbumes con paginación.
    /// </summary>
    /// <param name="offset">Número de elementos a omitir en la paginación.</param>
    /// <param name="limit">Número máximo de álbumes a devolver.</param>
    /// <returns>Una lista de objetos GetAlbumDTO con los álbumes disponibles.</returns>
    [HttpGet("getAlbums")]
    public async Task<ActionResult<List<GetAlbumDTO>>> GetAlbums(int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1) return BadRequest("Invalid offset or amount");
            var albums = await AlbumService.GetAllAlbums(offset, limit, _dbContext);
            return Ok(albums);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new { message = "Albums Not found", success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }
}
