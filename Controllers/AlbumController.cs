namespace purpuraMain.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;


/// Controlador para gestionar las operaciones relacionadas con los álbumes.
[ApiController]
[Route("[controller]")]
public class AlbumController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    /// Constructor del controlador AlbumController.
    /// <param name="dbContext">Contexto de base de datos para acceder a la información de los álbumes.</param>
    public AlbumController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// Obtiene un álbum por su identificador único.
    /// <param name="id">Identificador del álbum.</param>
    /// <returns>Un objeto GetAlbumDTO con los detalles del álbum.</returns>
    [HttpGet("getAlbum/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GetAlbumDTO>> GetAlbumById(string id)
    {
        try
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId)) userId = "0";
            var album = await AlbumService.GetAlbumById(userId, id, _dbContext);
            return Ok(album);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }


    /// Busca álbumes basándose en un texto de entrada.
    /// <param name="input">Texto a buscar en los álbumes.</param>
    /// <param name="offset">Número de elementos a omitir en la paginación.</param>
    /// <param name="limit">Número máximo de álbumes a devolver.</param>
    /// <returns>Una lista de objetos GetAlbumDTO con los álbumes encontrados.</returns>
    [HttpGet("getAlbumByInput/{input}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<GetAlbumDTO>>> GetAlbumByInput(string input, int offset, int limit)
    {
        try
        {
            var albums = await AlbumService.GetAlbumByInput(input, offset, limit, _dbContext);
            return Ok(albums);
        }
         catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }

    /// Obtiene una lista de álbumes con paginación.
    /// <param name="offset">Número de elementos a omitir en la paginación.</param>
    /// <param name="limit">Número máximo de álbumes a devolver.</param>
    /// <returns>Una lista de objetos GetAlbumDTO con los álbumes disponibles.</returns>
    [HttpGet("getAlbums")]
    [AllowAnonymous]
    public async Task<ActionResult<List<GetAlbumDTO>>> GetAlbums(int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1) return BadRequest("Invalid offset or amount");
            var albums = await AlbumService.GetAllAlbums(offset, limit, _dbContext);
            return Ok(albums);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }

    /// <summary>
    /// Obtiene los álbumes más populares del momento.
    /// </summary>
    /// <returns></returns>
    [HttpGet("getTopAlbums")]
    [AllowAnonymous]
    public async Task<ActionResult<List<GetAlbumDTO>>> GetTopAlbums()
    {
        try
        {
            var albums = await AlbumService.GetTopAlbums(_dbContext);
            return Ok(albums);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }
}
