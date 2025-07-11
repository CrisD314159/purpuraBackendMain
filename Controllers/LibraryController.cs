using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Controllers;


/// Controlador para la gestión de la biblioteca de música del usuario.
/// Constructor del controlador de biblioteca.
/// <param name="dbcontext">Contexto de la base de datos.</param>
[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
public class LibraryController(ILibraryService libraryService) : ControllerBase
{
    private readonly ILibraryService _libraryService = libraryService;

  /// Obtiene la biblioteca del usuario autenticado.
  /// <returns>Biblioteca del usuario.</returns>
  [HttpGet("user")]
    public async Task<IActionResult> GetLibrary()
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedException("You're not authorized to perform this action");
        GetLibraryDTO library = await _libraryService.GetLibraryById(userId);
        return Ok(library);

    }

    /// Obtiene las canciones de la biblioteca del usuario con paginación.
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Número de elementos a devolver.</param>
    /// <returns>Lista de canciones.</returns>
    [HttpGet("user/songs")]
    public async Task<IActionResult> GetUserSongs(int offset, int limit)
    {
        if (offset < 0 || limit < 1)
        {
            return BadRequest("Invalid offset or limit");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedException("You're not authorized to perform this action");
        var songs = await _libraryService.GetUserSongs(userId, offset, limit);
        return Ok(songs);

    }

    /// Agrega o elimina una canción a la biblioteca del usuario.
    [HttpPut("addSong")]
    public async Task<IActionResult> AddSong(AddRemoveSongLibraryDTO addSong)
    {
        return await ModifyLibrary(addSong, _libraryService.AddSongToLibrary, "Song added to library");
    }

    /// Agrega o elimina una playlist a la biblioteca del usuario.
    [HttpPut("addPlaylist")]
    public async Task<IActionResult> AddPlaylist(AddRemovePlayListDTO addPlaylist)
    {
        return await ModifyLibrary(addPlaylist, _libraryService.AddPlayListToLibrary, "Playlist added to library");
    }



    /// Agrega un álbum a la biblioteca del usuario.
    [HttpPut("addAlbum")]
    public async Task<IActionResult> AddAlbum(AddRemoveAlbumLibraryDTO addAlbum)
    {
        return await ModifyLibrary(addAlbum, _libraryService.AddAlbumToLibrary, "Album added to library");
    }

    /// Elimina un álbum de la biblioteca del usuario.
    [HttpPut("removeAlbum")]
    public async Task<IActionResult> RemoveAlbum(AddRemoveAlbumLibraryDTO removeAlbum)
    {
        return await ModifyLibrary(removeAlbum, _libraryService.AddAlbumToLibrary, "Album removed from library");
    }

    /// Método genérico para agregar o eliminar elementos de la biblioteca.
    private async Task<IActionResult> ModifyLibrary<T>(T dto, Func<string, T, Task> serviceMethod, string successMessage)
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User not found");
        await serviceMethod(userId, dto);
        return Ok(new { success = true, message = successMessage });

    }
}