namespace purpuraMain.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;


/// Controlador para gestionar las operaciones relacionadas con los álbumes.
/// Constructor del controlador AlbumController.
/// <param name="dbContext">Contexto de base de datos para acceder a la información de los álbumes.</param>
[ApiController]
[Route("[controller]")]
public class AlbumController(IAlbumService albumService) : ControllerBase
{
    private readonly IAlbumService _albumService = albumService;

    /// <summary>
    /// Creates an album with a provided create album dto 
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="createAlbumDTO"></param>
    /// <returns></returns>
    [Authorize(Roles ="ADMIN")]
    [HttpPost]
    public async Task<IActionResult> CreateAlbum(CreateAlbumDTO createAlbumDTO)
    {
        await _albumService.CreateAlbum(createAlbumDTO);
        return Created();
    }
    
    /// <summary>
    /// Updates an album with the provided update album dto
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="updateAlbumDTO"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpPut]
    public async Task<IActionResult> UpdateAlbum(UpdateAlbumDTO updateAlbumDTO)
    {
        await _albumService.UpdateAlbum(updateAlbumDTO);
        return Ok();
    }
    
    /// <summary>
    /// Deletes an album with the provided album Id
    /// This Endpint is only accessible for admins
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("/{id}")]
    public async Task<IActionResult> DeleteAlbum(Guid id)
    {
        await _albumService.DeleteAlbum(id);
        return Ok();
    }

  /// Obtiene un álbum por su identificador único.
  /// <param name="id">Identificador del álbum.</param>
  /// <returns>Un objeto GetAlbumDTO con los detalles del álbum.</returns>
    [HttpGet("getAlbum/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAlbumById(Guid id)
    {
        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(string.IsNullOrEmpty(userId)) userId = "0";
        var album = await _albumService.GetAlbumById(userId, id);
        return Ok(album);
    }


    /// Busca álbumes basándose en un texto de entrada.
    /// <param name="input">Texto a buscar en los álbumes.</param>
    /// <param name="offset">Número de elementos a omitir en la paginación.</param>
    /// <param name="limit">Número máximo de álbumes a devolver.</param>
    /// <returns>Una lista de objetos GetAlbumDTO con los álbumes encontrados.</returns>
    [HttpGet("getAlbumByInput/{input}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAlbumByInput(string input, int offset, int limit)
    {
 
        var albums = await _albumService.GetAlbumByInput(input, offset, limit);
        return Ok(albums);
   
    }

    /// Obtiene una lista de álbumes con paginación.
    /// <param name="offset">Número de elementos a omitir en la paginación.</param>
    /// <param name="limit">Número máximo de álbumes a devolver.</param>
    /// <returns>Una lista de objetos GetAlbumDTO con los álbumes disponibles.</returns>
    [HttpGet("getAlbums")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAlbums(int offset, int limit)
    {

        if (offset < 0 || limit < 1) return BadRequest("Invalid offset or amount");
        var albums = await _albumService.GetAllAlbums(offset, limit);
        return Ok(albums);

    }

    /// <summary>
    /// Obtiene los álbumes más populares del momento.
    /// </summary>
    /// <returns></returns>
    [HttpGet("getTopAlbums")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTopAlbums()
    {

        var albums = await _albumService.GetTopAlbums();
        return Ok(albums);

    }
}
