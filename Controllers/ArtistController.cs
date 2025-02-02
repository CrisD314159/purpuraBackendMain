namespace purpuraMain.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

/// <summary>
/// Controlador para la gestión de artistas en la plataforma.
/// Proporciona endpoints para obtener información sobre artistas, buscar artistas y recuperar sus álbumes y canciones.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class ArtistController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    /// <summary>
    /// Constructor del controlador de artistas.
    /// </summary>
    /// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el contexto es nulo.</exception>
    public ArtistController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Obtiene el perfil de un artista por su ID.
    /// </summary>
    /// <param name="id">ID del artista.</param>
    /// <returns>El perfil del artista.</returns>
    [HttpGet("getArtistProfile/{id}")]
    public async Task<ActionResult<GetArtistDTO>> GetArtistProfile(string id)
    {
        try
        {
            var artist = await ArtistService.GetArtistById(id, _dbContext);
            return Ok(artist);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new { message = "Artist Not found", success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }

    /// <summary>
    /// Busca artistas por nombre con paginación.
    /// </summary>
    /// <param name="name">Nombre del artista a buscar.</param>
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad de artistas a recuperar.</param>
    /// <returns>Lista de artistas coincidentes.</returns>
    [HttpGet("searchArtists")]
    public async Task<ActionResult<List<GetArtistDTO>>> GetArtistByName(string name, int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }

            var artist = await ArtistService.GetArtistByName(name, offset, limit, _dbContext);
            return Ok(artist);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new { message = "Artists Not found", success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }

    /// <summary>
    /// Obtiene los álbumes de un artista por su ID.
    /// </summary>
    /// <param name="id">ID del artista.</param>
    /// <returns>Lista de álbumes del artista.</returns>
    [HttpGet("getArtistAlbums/{id}")]
    public async Task<ActionResult<GetArtistDTO>> GetArtistAlbums(string id)
    {
        try
        {
            var artist = await ArtistService.GetArtistAlbums(id, _dbContext);
            return Ok(artist);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new { message = "Artist Not found", success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }

    /// <summary>
    /// Obtiene las canciones de un artista por su ID.
    /// </summary>
    /// <param name="id">ID del artista.</param>
    /// <returns>Lista de canciones del artista.</returns>
    [HttpGet("getArtistSongs/{id}")]
    public async Task<ActionResult<GetArtistDTO>> GetArtistSongs(string id)
    {
        try
        {
            var artist = await ArtistService.GetArtistById(id, _dbContext);
            return Ok(artist);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new { message = "Artist Not found", success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }

    /// <summary>
    /// Obtiene una lista de artistas con paginación.
    /// </summary>
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad de artistas a recuperar.</param>
    /// <returns>Lista de artistas.</returns>
    [HttpGet("getArtists")]
    public async Task<ActionResult<List<GetArtistDTO>>> GetArtists(int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }
            var artists = await ArtistService.GetMostListenArtists(offset, limit, _dbContext);
            return Ok(artists);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new { message = "Artists Not found", success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }

    /// <summary>
    /// Obtiene los artistas más escuchados con paginación.
    /// </summary>
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad de artistas a recuperar.</param>
    /// <returns>Lista de los artistas más escuchados.</returns>
    [HttpGet("getMostListenArtists")]
    public async Task<ActionResult<List<GetArtistPlaysDTO>>> GetMostListenArtists(int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }

            var artists = await ArtistService.GetMostListenArtists(offset, limit, _dbContext);
            return Ok(artists);
        }
        catch (EntityNotFoundException)
        {
            return NotFound(new { message = "Artists Not found", success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }
}