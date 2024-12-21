using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

[ApiController]
[Route("[controller]")]
public class SongController: ControllerBase
{

  private readonly PurpuraDbContext _dbContext;
  public SongController(PurpuraDbContext dbContext)
  {
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

  }


  [HttpGet("{id}")]
  public async Task<ActionResult<GetSongDTO>> GetSong(string id)
  {
    try
    {
      GetSongDTO song = await SongService.GetSongById(id, _dbContext) ?? throw new EntityNotFoundException("Song not found");
      return song;
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

  [HttpGet("search/songs/{input}")]
  public async Task<ActionResult<List<GetSongDTO>>> GetSongByInput(string input)
  {
    try
    {
      var song = await SongService.GetSongByInput(input, _dbContext) ?? throw new EntityNotFoundException("Song not found");
      return song;
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