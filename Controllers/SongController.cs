using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;


/// Controlador para gestionar las operaciones relacionadas con las canciones.
[ApiController]
[Route("[controller]")]
public class SongController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    /// Constructor del controlador de canciones.
    /// <param name="dbContext">Contexto de base de datos de la aplicación.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el contexto de la base de datos es nulo.</exception>
    public SongController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// Obtiene una canción por su identificador.
    /// <param name="id">Identificador de la canción.</param>
    /// <returns>Devuelve los detalles de la canción si se encuentra.</returns>
    [HttpGet("getSong/{id}")]
    [Authorize]
    public async Task<ActionResult<GetSongDTO>> GetSong(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});
            GetSongDTO song = await SongService.GetSongById(userId, id, _dbContext);
            return Ok(song);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }

    /// Busca canciones según un criterio de entrada.
    /// <param name="input">Texto de búsqueda.</param>
    /// <param name="offset">Número de registros a omitir.</param>
    /// <param name="limit">Número máximo de registros a devolver.</param>
    /// <returns>Lista de canciones coincidentes.</returns>
    [HttpGet("search/songs")]
    [Authorize]
    public async Task<ActionResult<List<GetSongDTO>>> GetSongByInput(string input, int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
            throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});
            var songs = await SongService.GetSongByInput(userId, input, offset, limit, _dbContext);
            return Ok(songs);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }

    /// Obtiene una lista paginada de canciones.
    /// <param name="offset">Número de registros a omitir.</param>
    /// <param name="limit">Número máximo de registros a devolver.</param>
    /// <returns>Lista de canciones disponibles.</returns>
    [HttpGet("getSongs")]
    [AllowAnonymous]
    public async Task<ActionResult<List<GetSongDTO>>> GetSongs(int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }

            var songs = await SongService.GetAllSongs(offset, limit, _dbContext);
            return Ok(songs);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }
    /// <summary>
    /// Obtiene una lista de canciones más populares.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet("getTopSongs")]
    [AllowAnonymous]
    public async Task<ActionResult<List<GetSongDTO>>> GetTopSongs()
    {
        try
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId)) userId = "0";
            var songs = await SongService.GetTopSongs(userId, _dbContext);
            return Ok(songs);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }

    /// <summary>
    /// Añade una reproducción a una canción.
    /// </summary>
    /// <param name="addPlayDTO"></param>
    /// <returns></returns>
    [HttpPost("addPlay")]
    [Authorize]
    public async Task<ActionResult<List<GetSongDTO>>> AddPlay(AddPlayDTO addPlayDTO)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            var songs = await SongService.AddSongPlay(userId, addPlayDTO.SongId, _dbContext);
            return Ok(songs);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }
}