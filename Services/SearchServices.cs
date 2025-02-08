using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;


namespace purpuraMain.Services;

public static class SearchServices
{

  public static async Task<GetSearchDTO> GetSearch(string userId, string input, PurpuraDbContext dbContext)
  {
    try
    {
      var results = new GetSearchDTO{
      Songs = await SongService.GetSongByInput(userId, input,0, 20, dbContext),
      Artists = await ArtistService.GetArtistByName(input, 0, 20, dbContext),
      Playlists = await PlaylistServices.SearchPlaylist(input, 0, 20, dbContext)
    };

    return results;
    }
    catch (System.Exception)
    {
      throw;
    }

  }
}