
namespace purpuraMain.Controllers;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Modes;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

[ApiController]
[Route("[controller]")]

public class ArtistController: ControllerBase
{
  private readonly PurpuraDbContext _dbContext;
  public ArtistController(PurpuraDbContext dbContext)
  {
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

  }

  [HttpGet("getArtistProfile/{id}")]
  public async Task<ActionResult<GetArtistDTO>> GetArtistProfile(string id)
  {
    try
    {
      var artist = await ArtistService.GetArtistById(id, _dbContext);
      return Ok(artist);
    }
    catch(EntityNotFoundException)
    {
      return NotFound("Artist Not found");
    }
    catch (System.Exception)
    {
      
      return BadRequest("An unexpected error occured");
    }

  }
  [HttpGet("searchArtists")]
  public async Task<ActionResult<List<GetArtistDTO>>> GetArtistByName(string name, int offset, int limit)
  {
    try
    {

      if(offset < 0 || limit < 1)
      {
        return BadRequest("Invalid offset or limit");
      }

      var artist = await ArtistService.GetArtistByName(name, offset, limit, _dbContext);
      return Ok(artist);
    }
    catch(EntityNotFoundException)
    {
      return NotFound("Artists Not found");
    }
    catch (System.Exception)
    {
      
      return BadRequest("An unexpected error occured");
    }

  }

  [HttpGet("getArtistAlbums/{id}")]
  public async Task<ActionResult<GetArtistDTO>> GetArtistAlbums(string id)
  {
    try
    {
      var artist = await ArtistService.GetArtistAlbums(id, _dbContext);
      return Ok(artist);
    }
    catch(EntityNotFoundException)
    {
      return NotFound("Artist Not found");
    }
    catch (System.Exception)
    {
      
      return BadRequest("An unexpected error occured");
    }

  }
  [HttpGet("getArtistSongs/{id}")]
  public async Task<ActionResult<GetArtistDTO>> GetArtistSongs(string id)
  {
    try
    {
      var artist = await ArtistService.GetArtistSongs(id, _dbContext);
      return Ok(artist);
    }
    catch(EntityNotFoundException)
    {
      return NotFound("Artist Not found");
    }
    catch (System.Exception)
    {
      
      return BadRequest("An unexpected error occured");
    }

  }

  [HttpGet("getArtists")]
  public async Task<ActionResult<List<GetArtistDTO>>> GetArtists(int offset, int limit)
  {
    try
    {
      if(offset < 0 || limit < 1)
      {
        return BadRequest("Invalid offset or limit");
      }
      var artists = await ArtistService.GetArtists(offset, limit, _dbContext);
      return Ok(artists);
    }
    catch(EntityNotFoundException)
    {
      return NotFound("Artists Not found");
    }
    catch (System.Exception)
    {
      
      throw;
    }
  }

}