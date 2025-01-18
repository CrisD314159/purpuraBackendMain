using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

[ApiController]
[Route("[controller]")]
public class AlbumController: ControllerBase
{
  private readonly PurpuraDbContext _dbContext;

  public AlbumController(PurpuraDbContext dbContext)
  {
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
  }



  [HttpGet("getAlbum/{id}")]
  public async Task<ActionResult<GetAlbumDTO>> GetAlbumById(string id)
  {

    try
    {
      var album = await AlbumService.GetAlbumById(id, _dbContext);
      return Ok(album);
    }
    catch(EntityNotFoundException)
    {
      return NotFound("Album Not found");
    }
    catch (System.Exception)
    {
      
      return BadRequest("An unexpected error occured");
    }
  }

  [HttpGet("getAlbumByInput/{input}")]
  public async Task<ActionResult<List<GetAlbumDTO>>> GetAlbumByInput(string input)
  {

    try
    {
      var album = await AlbumService.GetAlbumByInput(input, _dbContext);
      return Ok(album);
    }
    catch(EntityNotFoundException)
    {
      return NotFound("Albums Not found");
    }
    catch (System.Exception)
    {
      
      return BadRequest("An unexpected error occured");
    }
  }

  [HttpGet("getAlbums")]
  public async Task<ActionResult<List<GetAlbumDTO>>> GetAlbums(int offset, int limit)
  {
    try
    {
      if(offset < 0 || limit < 1) return BadRequest("Invalid offset or amount");
      

      var albums = await AlbumService.GetAllAlbumbs(offset, limit, _dbContext);
      return Ok(albums);
    }
    catch (System.Exception)
    {
      
      return BadRequest("An unexpected error occured");
    }
  }



}