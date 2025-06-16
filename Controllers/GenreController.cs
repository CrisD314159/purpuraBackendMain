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
/// Constructor del controlador de géneros.
/// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
[ApiController]
[Route("[controller]")]
[Authorize]
public class GenreController(IGenreService genreService) : ControllerBase
{
    private readonly IGenreService _genreService = genreService;
    
     /// <summary>
    /// Creates an genre with a provided create genre dto 
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="createArtistDTO"></param>
    /// <returns></returns>
    [Authorize(Roles ="ADMIN")]
    [HttpPost]
    public async Task<IActionResult> CreateGenre(CreateGenreDTO createGenreDTO)
    {
        await _genreService.CreateGenre(createGenreDTO);
        return Created();
    }
    
    /// <summary>
    /// Updates an genre with the provided update genre dto
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="updateGenreDTO"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut]
    public async Task<IActionResult> UpdateGenre(UpdateGenreDTO updateGenreDTO)
    {
        await _genreService.UpdateGenre(updateGenreDTO);
        return Ok();
    }
    
    /// <summary>
    /// Deletes an artist with the provided artist Id
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("/{id}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        await _genreService.DeleteGenre(id);
        return Ok();
    }


    /// Obtiene las canciones más populares de un género específico.
    /// <param name="id">ID del género.</param>
    /// <returns>Lista de canciones más populares dentro del género especificado.</returns>
    [HttpGet("getTopSongs/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopSongs(Guid id)
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
    public async Task<IActionResult> GetGenreById(Guid id)
    {

        var genre = await _genreService.GetGenreById(id);
        return Ok(genre);

    }
}