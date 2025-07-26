namespace purpuraMain.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.Dto.InputDto;
using purpuraMain.Services.Interfaces;

/// Controlador para la gestión de artistas en la plataforma.
/// Proporciona endpoints para obtener información sobre artistas, buscar artistas y recuperar sus álbumes y canciones.
/// Constructor del controlador de artistas.
/// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
/// <exception cref="ArgumentNullException">Se lanza si el contexto es nulo.</exception>
[ApiController]
[Route("[controller]")]
public class ArtistController(IArtistService artistService) : ControllerBase
{
    private readonly IArtistService _artistService = artistService;



    /// <summary>
    /// Creates an artist with a provided create artist dto 
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="createArtistDTO"></param>
    /// <returns></returns>
    [Authorize(Roles ="ADMIN", AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<IActionResult> CreateArtist(CreateArtistDTO createArtistDTO)
    {
        await _artistService.CreateArtist(createArtistDTO);
        return Created();
    }
    
    /// <summary>
    /// Updates an artist with the provided update artist dto
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="updateArtistDTO"></param>
    /// <returns></returns>
    [Authorize(Roles ="ADMIN", AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> UpdateAlbum(UpdateArtistDTO updateArtistDTO)
    {
        await _artistService.UpdateArtist(updateArtistDTO);
        return Ok();
    }
    
    /// <summary>
    /// Deletes an artist with the provided artist Id
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles ="ADMIN", AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArtist(Guid id)
    {
        await _artistService.DeleteArtist(id);
        return Ok();
    }

    /// Obtiene el perfil de un artista por su ID.
    /// <param name="id">ID del artista.</param>
    /// <returns>El perfil del artista.</returns>
    [HttpGet("getArtistProfile/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArtistProfile(Guid id)
    {

        var result = await HttpContext.AuthenticateAsync("Bearer");

        string userId = "0";
        if (result.Succeeded && result.Principal != null)
        {
            userId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        }
        var artist = await _artistService.GetArtistById(userId, id);
        return Ok(artist);

    }

   
    /// Busca artistas por nombre con paginación.
    /// <param name="name">Nombre del artista a buscar.</param>
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad de artistas a recuperar.</param>
    /// <returns>Lista de artistas coincidentes.</returns>
    [HttpGet("searchArtists")]
    public async Task<IActionResult> GetArtistByName(string name, int offset, int limit)
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
    public async Task<IActionResult> GetArtistAlbums(Guid id)
    {

        var artist = await _artistService.GetArtistAlbums(id);
        return Ok(artist);

    }


    /// Obtiene las canciones de un artista por su ID.
    /// <param name="id">ID del artista.</param>
    /// <returns>Lista de canciones del artista.</returns>
    [HttpGet("getArtistSongs/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArtistSongs(Guid id)
    {


        var result = await HttpContext.AuthenticateAsync("Bearer");

        string userId = "0";
        if (result.Succeeded && result.Principal != null)
        {
            userId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        }
        var artist = await _artistService.GetArtistById(userId, id);
        return Ok(artist);

    }


    /// Obtiene una lista de artistas con paginación.
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad de artistas a recuperar.</param>
    /// <returns>Lista de artistas.</returns>
    [HttpGet("getArtists")]
    [AllowAnonymous]
    public async Task<IActionResult> GetArtists(int offset, int limit)
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
    public async Task<IActionResult> GetMostListenArtists(int offset, int limit)
    {

            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }

            var artists = await _artistService.GetMostListenArtists(offset, limit);
            return Ok(artists);

    }
}