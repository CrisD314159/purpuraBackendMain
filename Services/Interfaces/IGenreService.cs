using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;


public interface IGenreService
{

  Task<GetGenreDTO> GetTopSongsByGenre(Guid genreId, string userId);

  Task<List<GetGenreDTO>> GetAllGenres();

  Task<GetGenreDTO> GetGenreById(Guid genreId);
  Task CreateGenre(CreateGenreDTO createGenreDTO);
  Task UpdateGenre(UpdateGenreDTO updateGenreDTO);
  Task DeleteGenre(Guid genreId);
}