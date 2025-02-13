using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;


/// Controlador para la gestión de playlists.
/// Permite crear, modificar, eliminar y gestionar canciones en playlists.
[ApiController]
[Route("[controller]")]
[Authorize]
public class PlaylistController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    public PlaylistController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// Obtiene una playlist por su ID.
    /// <param name="id">ID de la playlist.</param>
    /// <returns>Datos de la playlist.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPlayListDTO>> GetPlaylist(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            var playList = await PlaylistServices.GetPlaylist(userId, id, _dbContext) ?? throw new EntityNotFoundException("Playlist not found");
            return Ok(playList);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message, success = false });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message, success = false });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// Busca playlists por nombre con paginación.
    /// <param name="input">Texto de búsqueda.</param>
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Número de elementos a devolver.</param>
    /// <returns>Lista de playlists encontradas.</returns>
    [HttpGet("search/{input}")]
    public async Task<ActionResult<List<GetLibraryPlaylistDTO>>> SearchPlaylist(string input, [FromQuery] int offset, [FromQuery] int limit)
    {
        try
        {
            var playList = await PlaylistServices.SearchPlaylist(input, offset, limit, _dbContext) ?? throw new EntityNotFoundException("Playlist not found");
            return Ok(playList);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message, success = false });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// Agrega una canción a una playlist.
    /// <param name="addSongDTO">Datos de la canción y la playlist.</param>
    [HttpPut("addSong")]
    public async Task<ActionResult> AddSong(AddRemoveSongDTO addSongDTO)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            await PlaylistServices.AddSong(userId, addSongDTO, _dbContext);
            return Ok(new { message = "Song added to playlist", success = true });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// Elimina una canción de una playlist.
    /// <param name="addSongDTO">Datos de la canción y la playlist.</param>
    [HttpPut("removeSong")]
    public async Task<ActionResult> RemoveSong(AddRemoveSongDTO addSongDTO)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            await PlaylistServices.RemoveSong(userId, addSongDTO, _dbContext);
            return Ok(new { message = "Song removed from playlist", success = true });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// Cambia la privacidad de una playlist.
    /// <param name="changePrivacy">Datos de la playlist y la privacidad.</param>
    [HttpPut("changePrivacy")]
    public async Task<ActionResult> ChangePlaylistPrivacy(ChangePrivacyPlaylistDto changePrivacy)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            await PlaylistServices.ChangePlayListState(userId, changePrivacy, _dbContext);
            return Ok(new { message = "Playlist privacy changed", success = true });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// Obtiene las playlists del usuario autenticado.
    [HttpGet("getPlaylists/user")]
    public async Task<ActionResult<List<GetUserPlayListsDTO>>> GetUserPlaylists()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            var playLists = await PlaylistServices.GetUserPlayLists(userId, _dbContext);
            return Ok(playLists);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// Crea una nueva playlist.
    /// <param name="createPlayListDTO">Datos de la nueva playlist.</param>
    [HttpPost]
    public async Task<ActionResult> CreatePlayList(CreatePlayListDTO createPlayListDTO)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            await PlaylistServices.CreatePlayList(userId, createPlayListDTO, _dbContext);
            return Created("", new { message = $"Playlist {createPlayListDTO.Name} created", success = true });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// Actualiza una playlist existente.
    /// <param name="updatePlaylist">Datos de la playlist a actualizar.</param>
    [HttpPut]
    public async Task<ActionResult> UpdatePlaylist(UpdatePlaylistDTO updatePlaylist)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            await PlaylistServices.UpdatePlayList(userId, updatePlaylist, _dbContext);
            return Ok(new { message = "Playlist updated", success = true });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }

    /// Elimina una playlist.
    /// <param name="deletePlayList">Datos de la playlist a eliminar.</param>
    [HttpDelete]
    public async Task<ActionResult> DeletePlayList(DeletePlayListDTO deletePlayList)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
            await PlaylistServices.DeletePlayList(userId, deletePlayList, _dbContext);
            return Ok(new { message = "Playlist deleted", success = true });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, success = false });
        }
    }
}