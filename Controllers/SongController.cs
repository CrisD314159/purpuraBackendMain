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
public class SongController: ControllerBase
{

  private readonly PurpuraDbContext _dbContext;
  public SongController(PurpuraDbContext dbContext)
  {
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

  }


  [HttpGet("getSong/{id}")]
  public async Task<ActionResult<GetSongDTO>> GetSong(string id)
  {
    try
    {
      GetSongDTO song = await SongService.GetSongById(id, _dbContext) ?? throw new EntityNotFoundException("Song not found");
      return Ok(song);
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

  [HttpGet("search/songs")]
  public async Task<ActionResult<List<GetSongDTO>>> GetSongByInput(string input, int offset, int limit)
  {
    try
    {
      if(offset < 0 || limit < 1)
      {
        return BadRequest("Invalid offset or limit");
      }
      var song = await SongService.GetSongByInput(input, offset, limit, _dbContext) ?? throw new EntityNotFoundException("Song not found");
      return Ok(song);
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

  [HttpGet("getSongs")]
  public async Task<ActionResult<List<GetSongDTO>>> GetSongs(int offset, int limit)
  {
    try
    {
      if(offset < 0 || limit < 1)
      {
        return BadRequest("Invalid offset or limit");
      }
      var songs = await SongService.GetAllSongs(offset, limit, _dbContext) ?? throw new EntityNotFoundException("Song not found");
      return Ok(songs);
    }
    catch (EntityNotFoundException ex)
    {
      return NotFound(ex.Message);
    }
    catch (System.Exception)
    {
      
      throw;
    }
  }



}