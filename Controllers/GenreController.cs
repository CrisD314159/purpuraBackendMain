using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

[ApiController]
[Route("[controller]")]
public class GenreController : ControllerBase
{
  private readonly PurpuraDbContext _dbContext;
  public GenreController(PurpuraDbContext dbContext)
  {
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
  }

  [HttpGet("getTopSongs/{id}")]
  public async Task<ActionResult<GetGenreDTO>> GetTopSongs(string id)
  {
    try
    {
      var topSongs = await GenreService.GetTopSongsByGenre(id, _dbContext);
      return Ok(topSongs);
    }
     catch (EntityNotFoundException)
        {
            
            return NotFound("Genre Not found");
        }
        catch (System.Exception)
        {
            
            return BadRequest("An unexpected error occured");
        }
  }

  [HttpGet("getAll")]
  public async Task<ActionResult<List<GetGenreDTO>>> GetAllGenres()
  {
    try
    {
      var topSongs = await GenreService.GetAllGenres(_dbContext);
      return Ok(topSongs);
    }
    catch (System.Exception)
    {
            
      return BadRequest("An unexpected error occured");
    }
  }

}