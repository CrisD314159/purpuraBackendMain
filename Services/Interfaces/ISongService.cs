using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;


public interface ISongService
{

  Task<GetSongDTO> GetSongById(string userId, Guid songId);

  Task<List<GetSongDTO>> GetSongByInput(string userId, string input, int offset, int limit);

  Task<List<GetSongDTO>> GetAllSongs(int offset, int limit);

  Task<List<GetSongDTO>> GetTopSongs(string userId);

  Task<bool> AddSongPlay(string userId, Guid songId);
  Task CreateSong(CreateSingleSongDTO createSingleSongDTO);
  Task UpdateSong(UpdateSingleSongDTO updateSingleSongDTO);
  Task DeleteSong(Guid songId);

  Task AddSongToAlbum(AddSongToAlbumDTO addSongToAlbumDTO);
}