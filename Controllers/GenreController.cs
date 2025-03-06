namespace purpuraMain.Controllers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;


/// Controlador para gestionar géneros musicales.
[ApiController]
[Route("[controller]")]
[Authorize]
public class GenreController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    /// Constructor del controlador de géneros.
    /// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
    public GenreController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// Obtiene las canciones más populares de un género específico.
    /// <param name="id">ID del género.</param>
    /// <returns>Lista de canciones más populares dentro del género especificado.</returns>
    [HttpGet("getTopSongs/{id}")]
    public async Task<ActionResult<GetGenreDTO>> GetTopSongs(string id)
    {
        try
        {
            var topSongs = await GenreService.GetTopSongsByGenre(id, _dbContext);
            return Ok(topSongs);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }

    /// Obtiene todos los géneros disponibles en la plataforma.
    /// <returns>Lista de géneros musicales.</returns>
    [HttpGet("getAll")]
    public async Task<ActionResult<List<GetGenreDTO>>> GetAllGenres()
    {
        try
        {
            var genres = await GenreService.GetAllGenres(_dbContext);
            return Ok(genres);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }

    /// Obtiene la información de un género específico por su ID.
    /// <param name="id">ID del género.</param>
    /// <returns>Datos del género especificado.</returns>
    [HttpGet("getGenre/{id}")]
    public async Task<ActionResult<GetGenreDTO>> GetGenreById(string id)
    {
        try
        {
            var genre = await GenreService.GetGenreById(id, _dbContext);
            return Ok(genre);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }
}