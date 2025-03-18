using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Controllers;


/// Controlador para la gestión de la biblioteca de música del usuario.
[ApiController]
[Route("[controller]")]
[Authorize]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;

    /// Constructor del controlador de biblioteca.
    /// <param name="dbcontext">Contexto de la base de datos.</param>
    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    /// Obtiene la biblioteca del usuario autenticado.
    /// <returns>Biblioteca del usuario.</returns>
    [HttpGet("user")]
    public async Task<ActionResult<GetLibraryDTO>> GetLibrary()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException(401, new {Message ="You're not authorized to perform this action"});
            GetLibraryDTO library = await _libraryService.GetLibraryById(userId);
            return Ok(library);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }

    /// Obtiene las canciones de la biblioteca del usuario con paginación.
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Número de elementos a devolver.</param>
    /// <returns>Lista de canciones.</returns>
    [HttpGet("user/songs")]
    public async Task<ActionResult<GetLibraryDTO>> GetUserSongs(int offset, int limit)
    {
        try
        {
            if (offset < 0 || limit < 1)
            {
                return BadRequest("Invalid offset or limit");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException(401, new {Message = "You're not authorized to perform this action", Success = false});
            var songs = await _libraryService.GetUserSongs(userId, offset, limit);
            return Ok(songs);
        }
         catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }

    /// Agrega o elimina una canción a la biblioteca del usuario.
    [HttpPut("addSong")]
    public async Task<ActionResult> AddSong(AddRemoveSongLibraryDTO addSong)
    {
        return await ModifyLibrary(addSong, _libraryService.AddSongToLibrary, "Song added to library");
    }

    /// Agrega o elimina una playlist a la biblioteca del usuario.
    [HttpPut("addPlaylist")]
    public async Task<ActionResult> AddPlaylist(AddRemovePlayListDTO addPlaylist)
    {
        return await ModifyLibrary(addPlaylist, _libraryService.AddPlayListToLibrary, "Playlist added to library");
    }



    /// Agrega un álbum a la biblioteca del usuario.
    [HttpPut("addAlbum")]
    public async Task<ActionResult> AddAlbum(AddRemoveAlbumLibraryDTO addAlbum)
    {
        return await ModifyLibrary(addAlbum, _libraryService.AddAlbumToLibrary, "Album added to library");
    }

    /// Elimina un álbum de la biblioteca del usuario.
    [HttpPut("removeAlbum")]
    public async Task<ActionResult> RemoveAlbum(AddRemoveAlbumLibraryDTO removeAlbum)
    {
        return await ModifyLibrary(removeAlbum, _libraryService.AddAlbumToLibrary, "Album removed from library");
    }

    /// Método genérico para agregar o eliminar elementos de la biblioteca.
    private async Task<ActionResult> ModifyLibrary<T>(T dto, Func<string, T, Task> serviceMethod, string successMessage)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            await serviceMethod(userId, dto);
            return Ok(new { success = true, message = successMessage });
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }
}