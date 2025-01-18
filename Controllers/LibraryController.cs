using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

[ApiController]
[Route("[controller]")]
public class LibraryController : ControllerBase
{

  private readonly PurpuraDbContext _dbcontext;
  public LibraryController(PurpuraDbContext dbcontext)
  {
    _dbcontext = dbcontext ?? throw new ArgumentNullException(nameof(dbcontext));
  }


  [HttpGet("{id}")]
  public async Task<ActionResult<GetLibraryDTO>> GetLibrary(string id)
  {
    try
    {
      GetLibraryDTO library = await LibraryService.GetLibraryById(id, _dbcontext) ?? throw new EntityNotFoundException("Library not found");
      return Ok(library);
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
      await LibraryService.AddSongToLibrary(addSong, _dbcontext);
      return Ok("Song added to library");
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
      await LibraryService.AddSongToLibrary(addSong, _dbcontext);
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
      await LibraryService.AddPlayListToLibrary(addPlaylist, _dbcontext);
      return Ok("Playlist added to library");
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
      await LibraryService.AddPlayListToLibrary(addPlaylist, _dbcontext);
      return Ok("Playlist removed from library");
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
      await LibraryService.AddAlbumToLibrary(addAlbum, _dbcontext);
      return Ok("Album added to library");
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
      await LibraryService.AddAlbumToLibrary(addAlbum, _dbcontext);
      return Ok("Album removed to library");
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