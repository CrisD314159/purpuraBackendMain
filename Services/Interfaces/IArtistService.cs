using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;

public interface IArtistService
{
  Task<GetArtistDTO> GetArtistById(string userId, Guid artistId);

  Task<List<GetArtistDTO>> GetArtistByName(string name, int offset, int limit);

  Task<GetArtistDTO> GetArtistAlbums(Guid artistId);
  Task<List<GetArtistPlaysDTO>> GetMostListenArtists(int offset, int limit);
  Task<List<GetSongDTO>> GetTopArtistSongs(string userId, Guid artistId);

  Task CreateArtist(CreateArtistDTO createArtistDTO);
  Task UpdateArtist(UpdateArtistDTO updateArtistDTO);
  Task DeleteArtist(Guid artistId);
}