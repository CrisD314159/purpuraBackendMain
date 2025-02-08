using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

/// <summary>
/// Controlador para gestionar las operaciones relacionadas con las canciones.
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class SongController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    /// <summary>
    /// Constructor del controlador de canciones.
    /// </summary>
    /// <param name="dbContext">Contexto de base de datos de la aplicación.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el contexto de la base de datos es nulo.</exception>
    public SongController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Obtiene una canción por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la canción.</param>
    /// <returns>Devuelve los detalles de la canción si se encuentra.</returns>
    [HttpGet("getSong/{id}")]
    public async Task<ActionResult<GetSongDTO>> GetSong(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            GetSongDTO song = await SongService.GetSongById(userId, id, _dbContext) 
                ?? throw new EntityNotFoundException("Song not found");
            return Ok(song);
        }
        catch(UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message, success = false });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message, success = false });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// <summary>
    /// Busca canciones según un criterio de entrada.
    /// </summary>
    /// <param name="input">Texto de búsqueda.</param>
    /// <param name="offset">Número de registros a omitir.</param>
    /// <param name="limit">Número máximo de registros a devolver.</param>
    /// <returns>Lista de canciones coincidentes.</returns>
    [HttpGet("search/songs")]
    public async Task<ActionResult<List<GetSongDTO>>> GetSongByInput(string input, int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            var songs = await SongService.GetSongByInput(userId, input, offset, limit, _dbContext) 
                ?? throw new EntityNotFoundException("Song not found");
            return Ok(songs);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message, success = false });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// <summary>
    /// Obtiene una lista paginada de canciones.
    /// </summary>
    /// <param name="offset">Número de registros a omitir.</param>
    /// <param name="limit">Número máximo de registros a devolver.</param>
    /// <returns>Lista de canciones disponibles.</returns>
    [HttpGet("getSongs")]
    public async Task<ActionResult<List<GetSongDTO>>> GetSongs(int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }

            var songs = await SongService.GetAllSongs(offset, limit, _dbContext) 
                ?? throw new EntityNotFoundException("Song not found");
            return Ok(songs);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message, success = false });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }
}