namespace purpuraMain.Services.Implementations;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Services.Interfaces;

public class SearchService(IArtistService artistService, ISongService songService) : ISearchService
{
  private readonly IArtistService _artistService = artistService;
  private readonly ISongService _songService = songService;

  /// <summary>
  /// Obtiene las canciones, artistas y playlists que coincidan con la búsqueda.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="input"></param>
  /// <param name="dbContext"></param>
  /// <returns></returns>
  public async Task<GetSearchDTO> GetSearch(string userId, string input)
  {

      var results = new GetSearchDTO{
      Songs = await _songService.GetSongByInput(userId, input,0, 20),
      Artists = await _artistService.GetArtistByName(input, 0, 20)
    };

    return results;
  }
}