using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

[ApiController]
[Route("[controller]")]
public class PlaylistController : ControllerBase
{

  private readonly PurpuraDbContext _dbContext;

  public PlaylistController (PurpuraDbContext dbContext)
  {
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); 

  }

  [HttpGet("{id}")]
  public async Task<ActionResult<GetPlayListDTO>> GetPlaylist(string id)
  {
    try
    {
      GetPlayListDTO playList = await PlaylistServices.GetPlaylist(id, _dbContext) ?? throw new EntityNotFoundException("Playlist not found");
      return playList;
    }
    catch(ValidationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch(EntityNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
    catch (System.Exception e)
    {
      return BadRequest(e.Message);
    }
  }
  [HttpPut("addSong")]
  public async Task<ActionResult> AddSong(AddRemoveSongDTO addSongDTO)
  {
    try
    {
      var playList = await PlaylistServices.AddSong(addSongDTO, _dbContext);
      return Ok("Song added to playlist");
    }
     catch(ValidationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch(EntityNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
    catch (System.Exception e)
    {
      return BadRequest(e.Message);
    }
  }
  [HttpPut("removeSong")]
  public async Task<ActionResult> Remove(AddRemoveSongDTO addSongDTO)
  {
    try
    {
      var playList = await PlaylistServices.RemoveSong(addSongDTO, _dbContext);
      return Ok("Song Removed from playlist");
    }
     catch(ValidationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch(EntityNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
    catch (System.Exception e)
    {
      return BadRequest(e.Message);
    }
  }

   [HttpPut("changePrivacy")]
  public async Task<ActionResult> ChangePlaylistPrivacy(ChangePrivacyPlaylistDto changePrivacy)
  {
    try
    {
      var playList = await PlaylistServices.ChangePlayListState(changePrivacy, _dbContext);
      return Ok("Playlist privacy changed");
    }
     catch(ValidationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch(EntityNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
    catch (System.Exception e)
    {
      return BadRequest(e.Message);
    }
  }
   [HttpGet("getPlaylists/user/{id}")]
  public async Task<ActionResult<List<GetUserPlayListsDTO>>> GetUserPlaylists(string id)
  {
    try
    {
      var playLists = await PlaylistServices.GetUserPlayLists(id, _dbContext);
      return playLists;
    }
    catch(EntityNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
     catch(ValidationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (System.Exception e)
    {
      return BadRequest(e.Message);
    }
  }
   [HttpPost]
  public async Task<ActionResult> CreatePlayList(CreatePlayListDTO createPlayListDTO)
  {
    try
    {
      var playList = await PlaylistServices.CreatePlayList(createPlayListDTO, _dbContext);
      return Created("Playlist created", new {message = $"Playlist {createPlayListDTO.Name} created",});
    }
    catch(EntityNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
    catch(ValidationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (System.Exception e)
    {
      return BadRequest(e.Message);
    }
  }

   [HttpPut]
  public async Task<ActionResult> UpdatePlaylist(UpdatePlaylistDTO updatePlaylist)
  {
    try
    {
      var playList = await PlaylistServices.UpdatePlayList(updatePlaylist, _dbContext);
      return Ok("Playlist updated");
    }
    catch(EntityNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
    catch(ValidationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (System.Exception e)
    {
      return BadRequest(e.Message);
    }
  }
   [HttpDelete]
  public async Task<ActionResult> DeletePlayList(DeletePlayListDTO deletePlayList)
  {
    try
    {
      var playList = await PlaylistServices.DeletePlayList(deletePlayList, _dbContext);
      return Ok("Playlist deleted");
    }
    catch(EntityNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
     catch(ValidationException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (System.Exception e)
    {
      return BadRequest(e.Message);
    }
  }


}