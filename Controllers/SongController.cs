using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Controllers;


/// Controlador para gestionar las operaciones relacionadas con las canciones.
/// Constructor del controlador de canciones.
/// <param name="dbContext">Contexto de base de datos de la aplicación.</param>
/// <exception cref="ArgumentNullException">Se lanza si el contexto de la base de datos es nulo.</exception>
[ApiController]
[Route("[controller]")]
public class SongController(ISongService ISongService) : ControllerBase
{
    private readonly ISongService _songService = ISongService;
    
    /// <summary>
    /// Creates an song with a provided create song dto 
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="createSingleSongDTO"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<IActionResult> CreateSong(CreateSingleSongDTO createSingleSongDTO)
    {
        await _songService.CreateSong(createSingleSongDTO);
        return Created();
    }

    /// <summary>
    /// Creates an adds a new song to an existing album
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="addSongToAlbumDTO"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("/addToAlbum")]
    public async Task<IActionResult> AddSongToAlbum(AddSongToAlbumDTO addSongToAlbumDTO)
    {
        await _songService.AddSongToAlbum(addSongToAlbumDTO);
        return Created();
    }
    
    /// <summary>
    /// Updates a song with the provided update song dto
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="updateGenreDTO"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut]
    public async Task<IActionResult> UpdateSong(UpdateSingleSongDTO updateSingleSongDTO)
    {
        await _songService.UpdateSong(updateSingleSongDTO);
        return Ok();
    }
    
    /// <summary>
    /// Deletes a song with the provided song Id
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("/{id}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        await _songService.DeleteSong(id);
        return Ok();
    }


  /// Obtiene una canción por su identificador.
    /// <param name="id">Identificador de la canción.</param>
    /// <returns>Devuelve los detalles de la canción si se encuentra.</returns>
    [HttpGet("getSong/{id}")]
    [Authorize]
    public async Task<IActionResult> GetSong(Guid id)
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("You're not authorized to perform this action");
        GetSongDTO song = await _songService.GetSongById(userId, id);
        return Ok(song);

    }

    /// Busca canciones según un criterio de entrada.
    /// <param name="input">Texto de búsqueda.</param>
    /// <param name="offset">Número de registros a omitir.</param>
    /// <param name="limit">Número máximo de registros a devolver.</param>
    /// <returns>Lista de canciones coincidentes.</returns>
    [HttpGet("search/songs")]
    [Authorize]
    public async Task<IActionResult> GetSongByInput(string input, int offset, int limit)
    {

        if (offset < 0 || limit < 1)
        {
            return BadRequest("Invalid offset or limit");
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
        throw new UnauthorizedException("You're not authorized to perform this action");
        var songs = await _songService.GetSongByInput(userId, input, offset, limit);
        return Ok(songs);

    }

    /// Obtiene una lista paginada de canciones.
    /// <param name="offset">Número de registros a omitir.</param>
    /// <param name="limit">Número máximo de registros a devolver.</param>
    /// <returns>Lista de canciones disponibles.</returns>
    [HttpGet("getSongs")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSongs(int offset, int limit)
    {
 
        if (offset < 0 || limit < 1)
        {
            return BadRequest("Invalid offset or limit");
        }

        var songs = await _songService.GetAllSongs(offset, limit);
        return Ok(songs);
    }
    /// <summary>
    /// Obtiene una lista de canciones más populares.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet("getTopSongs")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopSongs()
    {

        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(string.IsNullOrEmpty(userId)) userId = "0";
        var songs = await _songService.GetTopSongs(userId);
        return Ok(songs);
        
    }

    /// <summary>
    /// Añade una reproducción a una canción.
    /// </summary>
    /// <param name="addPlayDTO"></param>
    /// <returns></returns>
    [HttpPost("addPlay")]
    [Authorize]
    public async Task<IActionResult> AddPlay(AddPlayDTO addPlayDTO)
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
        var songs = await _songService.AddSongPlay(userId, addPlayDTO.SongId);
        return Ok(songs);

    }
}