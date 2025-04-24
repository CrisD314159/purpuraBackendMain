namespace purpuraMain.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;

/// Controlador para la gestión de artistas en la plataforma.
/// Proporciona endpoints para obtener información sobre artistas, buscar artistas y recuperar sus álbumes y canciones.
[ApiController]
[Route("[controller]")]
public class ArtistController : ControllerBase
{
    private readonly IArtistService _artistService;

   
    /// Constructor del controlador de artistas.
    /// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
    /// <exception cref="ArgumentNullException">Se lanza si el contexto es nulo.</exception>
    public ArtistController(IArtistService artistService)
    {
        _artistService = artistService;
    }

   
    /// Obtiene el perfil de un artista por su ID.
    /// <param name="id">ID del artista.</param>
    /// <returns>El perfil del artista.</returns>
    [HttpGet("getArtistProfile/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GetArtistDTO>> GetArtistProfile(string id)
    {
             var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId)) userId = "0";
            var artist = await _artistService.GetArtistById(userId, id);
            return Ok(artist);

    }

   
    /// Busca artistas por nombre con paginación.
    /// <param name="name">Nombre del artista a buscar.</param>
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad de artistas a recuperar.</param>
    /// <returns>Lista de artistas coincidentes.</returns>
    [HttpGet("searchArtists")]
    public async Task<ActionResult<List<GetArtistDTO>>> GetArtistByName(string name, int offset, int limit)
    {

            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }

            var artist = await _artistService.GetArtistByName(name, offset, limit);
            return Ok(artist);

    }

   
    /// Obtiene los álbumes de un artista por su ID.
    /// <param name="id">ID del artista.</param>
    /// <returns>Lista de álbumes del artista.</returns>
    [HttpGet("getArtistAlbums/{id}")]
    public async Task<ActionResult<GetArtistDTO>> GetArtistAlbums(string id)
    {

            var artist = await _artistService.GetArtistAlbums(id);
            return Ok(artist);

    }


    /// Obtiene las canciones de un artista por su ID.
    /// <param name="id">ID del artista.</param>
    /// <returns>Lista de canciones del artista.</returns>
    [HttpGet("getArtistSongs/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GetArtistDTO>> GetArtistSongs(string id)
    {

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId)) userId = "0";
            var artist = await _artistService.GetArtistById(userId, id);
            return Ok(artist);

    }

   
    /// Obtiene una lista de artistas con paginación.
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad de artistas a recuperar.</param>
    /// <returns>Lista de artistas.</returns>
    [HttpGet("getArtists")]
    public async Task<ActionResult<List<GetArtistDTO>>> GetArtists(int offset, int limit)
    {

            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }
            var artists = await _artistService.GetMostListenArtists(offset, limit);
            return Ok(artists);

    }

   
    /// Obtiene los artistas más escuchados con paginación.
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad de artistas a recuperar.</param>
    /// <returns>Lista de los artistas más escuchados.</returns>
    [HttpGet("getMostListenArtists")]
    public async Task<ActionResult<List<GetArtistPlaysDTO>>> GetMostListenArtists(int offset, int limit)
    {

            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }

            var artists = await _artistService.GetMostListenArtists(offset, limit);
            return Ok(artists);

    }
}