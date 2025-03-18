using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;

public interface ILibraryService
{
  Task<GetLibraryDTO> GetLibraryById(string userId);

  Task<GetLibraryDTO> GetUserSongs (string userId, int offset, int limit);

  Task<bool> AddSongToLibrary(string userId, AddRemoveSongLibraryDTO addRemoveSong);
 
  Task<bool> AddAlbumToLibrary(string userId, AddRemoveAlbumLibraryDTO addRemoveAlbum);

   
  Task<bool> AddPlayListToLibrary(string userId, AddRemovePlayListDTO addRemovePlayListDTO);
}