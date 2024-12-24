
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
      return artist;
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
  [HttpGet("searchArtists/{name}")]
  public async Task<ActionResult<List<GetArtistDTO>>> GetArtistByName(string name)
  {
    try
    {
      var artist = await ArtistService.GetArtistByName(name, _dbContext);
      return artist;
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
      return artist;
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
      return artist;
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

}