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

namespace purpuraMain.Controllers;


/// Controlador para la gestión de playlists.
/// Permite crear, modificar, eliminar y gestionar canciones en playlists.
[ApiController]
[Route("[controller]")]
[Authorize]
public class PlaylistController : ControllerBase
{
    private readonly IPlaylistService _playlistService;

    public PlaylistController(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    /// Obtiene una playlist por su ID.
    /// <param name="id">ID de la playlist.</param>
    /// <returns>Datos de la playlist.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPlayListDTO>> GetPlaylist(string id)
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException(" You're not authorized to perform this action");
            var playList = await _playlistService.GetPlaylist(userId, id);
            return Ok(playList);
        
    }

    /// Busca playlists por nombre con paginación.
    /// <param name="input">Texto de búsqueda.</param>
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Número de elementos a devolver.</param>
    /// <returns>Lista de playlists encontradas.</returns>
    [HttpGet("search/{input}")]
    public async Task<ActionResult<List<GetLibraryPlaylistDTO>>> SearchPlaylist(string input, [FromQuery] int offset, [FromQuery] int limit)
    {

            var playList = await _playlistService.SearchPlaylist(input, offset, limit);
            return Ok(playList);

    }

    /// Agrega una canción a una playlist.
    /// <param name="addSongDTO">Datos de la canción y la playlist.</param>
    [HttpPut("addSong")]
    public async Task<ActionResult> AddSong(AddRemoveSongDTO addSongDTO)
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException(" You're not authorized to perform this action");
            await _playlistService.AddSong(userId, addSongDTO);
            return Ok(new { message = "Song added to playlist", success = true });
 
    }

    /// Elimina una canción de una playlist.
    /// <param name="addSongDTO">Datos de la canción y la playlist.</param>
    [HttpPut("removeSong")]
    public async Task<ActionResult> RemoveSong(AddRemoveSongDTO addSongDTO)
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("You're not authorized to perform this action");
            await _playlistService.RemoveSong(userId, addSongDTO);
            return Ok(new { message = "Song removed from playlist", success = true });

    }

    /// Cambia la privacidad de una playlist.
    /// <param name="changePrivacy">Datos de la playlist y la privacidad.</param>
    [HttpPut("changePrivacy")]
    public async Task<ActionResult> ChangePlaylistPrivacy(ChangePrivacyPlaylistDto changePrivacy)
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("You're not authorized to perform this action");
            await _playlistService.ChangePlayListState(userId, changePrivacy);
            return Ok(new { message = "Playlist privacy changed", success = true });

    }

    /// Obtiene las playlists del usuario autenticado.
    [HttpGet("getPlaylists/user")]
    public async Task<ActionResult<List<GetUserPlayListsDTO>>> GetUserPlaylists()
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("You're not authorized to perform this action");
            var playLists = await _playlistService.GetUserPlayLists(userId);
            return Ok(playLists);

    }

    /// Crea una nueva playlist.
    /// <param name="createPlayListDTO">Datos de la nueva playlist.</param>
    [HttpPost]
    public async Task<ActionResult> CreatePlayList(CreatePlayListDTO createPlayListDTO)
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("You're not authorized to perform this action");
            await _playlistService.CreatePlayList(userId, createPlayListDTO);
            return Created("", new { message = $"Playlist {createPlayListDTO.Name} created", success = true });

    }

    /// Actualiza una playlist existente.
    /// <param name="updatePlaylist">Datos de la playlist a actualizar.</param>
    [HttpPut]
    public async Task<ActionResult> UpdatePlaylist(UpdatePlaylistDTO updatePlaylist)
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("You're not authorized to perform this action");
            await _playlistService.UpdatePlayList(userId, updatePlaylist);
            return Ok(new { message = "Playlist updated", success = true });

    }

    /// Elimina una playlist.
    /// <param name="deletePlayList">Datos de la playlist a eliminar.</param>
    [HttpDelete]
    public async Task<ActionResult> DeletePlayList(DeletePlayListDTO deletePlayList)
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("You're not authorized to perform this action");
            await _playlistService.DeletePlayList(userId, deletePlayList);
            return Ok(new { message = "Playlist deleted", success = true });

    }
}