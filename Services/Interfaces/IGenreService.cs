using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;


public interface IGenreService
{

  Task<GetGenreDTO> GetTopSongsByGenre(string id, string userId);

  Task<List<GetGenreDTO>> GetAllGenres();
    
  Task<GetGenreDTO> GetGenreById(string id);
}