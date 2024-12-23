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
      return topSongs;
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

  [HttpGet]
  public async Task<ActionResult<List<GetGenreDTO>>> GetAllGenres(string id)
  {
    try
    {
      var topSongs = await GenreService.GetAllGenres(_dbContext);
      return topSongs;
    }
    catch (System.Exception)
    {
            
      return BadRequest("An unexpected error occured");
    }
  }
  [HttpPost]
  public async Task<ActionResult> CreateGenre(CreateGenreDTO createGenre)
  {
    try
    {
      await GenreService.CreateGenre(createGenre,_dbContext);
      return Created("Genre successfully created", new {message = $"Genre {createGenre.Name} successfully created"});
    }
    catch(ValidationException ex)
    {
      return BadRequest(ex.Message);

    }
    catch (System.Exception)
    {  
      return BadRequest("An unexpected error occured");
    }
  }
  [HttpPut]
  public async Task<ActionResult> UpdateGenre(UpdateGenreDTO updateGenre)
  {
    try
    {
      await GenreService.UpdateGenre(updateGenre,_dbContext);
      return Ok("Genre successfully updated");
    }
    catch(ValidationException ex)
    {
      return BadRequest(ex.Message);

    }
    catch(EntityNotFoundException)
    {
      return NotFound("Genre Not Found");
    }
    catch (System.Exception)
    {  
      return BadRequest("An unexpected error occured");
    }
  }
  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteGenre(string id)
  {
    try
    {
      await GenreService.DeleteGenre(id,_dbContext);
      return Ok("Genre successfully deleted");
    }
    catch(EntityNotFoundException)
    {
      return NotFound("Genre Not Found");
    }
    catch (System.Exception)
    {  
      return BadRequest("An unexpected error occured");
    }
  }
}