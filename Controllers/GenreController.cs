namespace purpuraMain.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using purpuraMain.Services.Interfaces;


/// Controlador para gestionar géneros musicales.
[ApiController]
[Route("[controller]")]
[Authorize]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;

    /// Constructor del controlador de géneros.
    /// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    /// Obtiene las canciones más populares de un género específico.
    /// <param name="id">ID del género.</param>
    /// <returns>Lista de canciones más populares dentro del género especificado.</returns>
    [HttpGet("getTopSongs/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopSongs(string id)
    {
        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(string.IsNullOrEmpty(userId)) userId = "0";
            var topSongs = await _genreService.GetTopSongsByGenre(id, userId);
            return Ok(topSongs);

    }

    /// Obtiene todos los géneros disponibles en la plataforma.
    /// <returns>Lista de géneros musicales.</returns>
    [HttpGet("getAll")]
    public async Task<IActionResult> GetAllGenres()
    {

        var genres = await _genreService.GetAllGenres();
        return Ok(genres);

    }

    /// Obtiene la información de un género específico por su ID.
    /// <param name="id">ID del género.</param>
    /// <returns>Datos del género especificado.</returns>
    [HttpGet("getGenre/{id}")]
    public async Task<IActionResult> GetGenreById(string id)
    {

        var genre = await _genreService.GetGenreById(id);
        return Ok(genre);

    }
}