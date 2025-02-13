using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;


/// Controlador para la gestión de la biblioteca de música del usuario.
[ApiController]
[Route("[controller]")]
[Authorize]
public class LibraryController : ControllerBase
{
    private readonly PurpuraDbContext _dbcontext;

    /// Constructor del controlador de biblioteca.
    /// <param name="dbcontext">Contexto de la base de datos.</param>
    public LibraryController(PurpuraDbContext dbcontext)
    {
        _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
    }

    /// Obtiene la biblioteca del usuario autenticado.
    /// <returns>Biblioteca del usuario.</returns>
    [HttpGet("user")]
    public async Task<ActionResult<GetLibraryDTO>> GetLibrary()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            GetLibraryDTO library = await LibraryService.GetLibraryById(userId, _dbcontext) ?? throw new EntityNotFoundException("Library not found");
            return Ok(library);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
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

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            var songs = await LibraryService.GetUserSongs(userId, offset, limit, _dbcontext) ?? throw new EntityNotFoundException("Library not found");
            return Ok(songs);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message, success = false });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message, success = false });
        }
        catch (System.Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// Agrega o elimina una canción a la biblioteca del usuario.
    [HttpPut("addSong")]
    public async Task<ActionResult> AddSong(AddRemoveSongLibraryDTO addSong)
    {
        return await ModifyLibrary(addSong, LibraryService.AddSongToLibrary, "Song added to library");
    }

    /// Agrega o elimina una playlist a la biblioteca del usuario.
    [HttpPut("addPlaylist")]
    public async Task<ActionResult> AddPlaylist(AddRemovePlayListDTO addPlaylist)
    {
        return await ModifyLibrary(addPlaylist, LibraryService.AddPlayListToLibrary, "Playlist added to library");
    }



    /// Agrega un álbum a la biblioteca del usuario.
    [HttpPut("addAlbum")]
    public async Task<ActionResult> AddAlbum(AddRemoveAlbumLibraryDTO addAlbum)
    {
        return await ModifyLibrary(addAlbum, LibraryService.AddAlbumToLibrary, "Album added to library");
    }

    /// Elimina un álbum de la biblioteca del usuario.
    [HttpPut("removeAlbum")]
    public async Task<ActionResult> RemoveAlbum(AddRemoveAlbumLibraryDTO removeAlbum)
    {
        return await ModifyLibrary(removeAlbum, LibraryService.AddAlbumToLibrary, "Album removed from library");
    }

    /// Método genérico para agregar o eliminar elementos de la biblioteca.
    private async Task<ActionResult> ModifyLibrary<T>(T dto, Func<string, T, PurpuraDbContext, Task> serviceMethod, string successMessage)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            await serviceMethod(userId, dto, _dbcontext);
            return Ok(new { success = true, message = successMessage });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message, success = false });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message, success = false });
        }
        catch (System.Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }
}