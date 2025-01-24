using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class LibraryController : ControllerBase
{

  private readonly PurpuraDbContext _dbcontext;
  public LibraryController(PurpuraDbContext dbcontext)
  {
    _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
  }


  [HttpGet("user")]
  public async Task<ActionResult<GetLibraryDTO>> GetLibrary()
  {
    try
    {
      // Requires userId extraction from token
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
      GetLibraryDTO library = await LibraryService.GetLibraryById(userId, _dbcontext) ?? throw new EntityNotFoundException("Library not found");
      return Ok(library);
    }
    catch( UnauthorizedAccessException ex){
      return Unauthorized(ex.Message);
    }
    catch (ValidationException ex)
    {
      return BadRequest(ex.Message);
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

  [HttpPut("addSong")]
  public async Task<ActionResult> AddSong(AddRemoveSongLibraryDTO addSong )
  {
    try
    {
      // Requires userId extraction from token
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
      await LibraryService.AddSongToLibrary(userId, addSong, _dbcontext);
      return Ok("Song added to library");
    }
    catch( UnauthorizedAccessException ex){
      return Unauthorized(ex.Message);
    }
    catch (ValidationException ex)
    {
      return BadRequest(ex.Message);
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
  
  [HttpPut("removeSong")]
  public async Task<ActionResult> RemoveSong(AddRemoveSongLibraryDTO addSong )
  {
    try
    {
      // Requires userId extraction from token
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
      await LibraryService.AddSongToLibrary(userId, addSong, _dbcontext);
      return Ok("Song removed from library");
    }
    catch (ValidationException ex)
    {
      return BadRequest(ex.Message);
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
  [HttpPut("addPlaylist")]
  public async Task<ActionResult> AddPlayList(AddRemovePlayListDTO addPlaylist )
  {
    try
    {
      // Requires userId extraction from token
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found"); 
      await LibraryService.AddPlayListToLibrary(userId, addPlaylist, _dbcontext);
      return Ok("Playlist added to library");
    }
     catch( UnauthorizedAccessException ex){
      return Unauthorized(ex.Message);
    }
    catch (ValidationException ex)
    {
      return BadRequest(ex.Message);
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

  [HttpPut("removePlaylist")]
  public async Task<ActionResult> RemovePlayList(AddRemovePlayListDTO addPlaylist )
  {
    try
    {
      // Requires userId extraction from token
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
      await LibraryService.AddPlayListToLibrary(userId, addPlaylist, _dbcontext);
      return Ok("Playlist removed from library");
    }
     catch( UnauthorizedAccessException ex){
      return Unauthorized(ex.Message);
    }
    catch (ValidationException ex)
    {
      return BadRequest(ex.Message);
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
  [HttpPut("addAlbum")]
  public async Task<ActionResult> AddAlbum(AddRemoveAlbumLibraryDTO addAlbum )
  {
    try
    {
      // Requires userId extraction from token
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
      await LibraryService.AddAlbumToLibrary(userId,addAlbum, _dbcontext);
      return Ok("Album added to library");
    }
     catch( UnauthorizedAccessException ex){
      return Unauthorized(ex.Message);
    }
    catch (ValidationException ex)
    {
      return BadRequest(ex.Message);
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
  [HttpPut("removeAlbum")]
  public async Task<ActionResult> RemoveAlbum(AddRemoveAlbumLibraryDTO addAlbum )
  {
    try
    {
      // Requires userId extraction from token
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
      await LibraryService.AddAlbumToLibrary(userId, addAlbum, _dbcontext);
      return Ok("Album removed to library");
    }
     catch( UnauthorizedAccessException ex){
      return Unauthorized(ex.Message);
    }
    catch (ValidationException ex)
    {
      return BadRequest(ex.Message);
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
 

}