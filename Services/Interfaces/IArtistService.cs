using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;

public interface IArtistService
{
  Task<GetArtistDTO> GetArtistById(string userId, string id);

  Task<List<GetArtistDTO>> GetArtistByName(string name, int offset, int limit);

  Task<GetArtistDTO> GetArtistAlbums(string id);

  Task<List<GetArtistPlaysDTO>> GetMostListenArtists(int offset, int limit);
  Task<List<GetSongDTO>> GetTopArtistSongs(string userId, string id);
}