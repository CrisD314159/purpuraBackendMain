using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Model;

namespace purpuraMain.Services.Interfaces;

public interface ILibraryService
{
  Task<GetLibraryDTO> GetLibraryById(string userId);

  Task<GetLibraryDTO> GetUserSongs(string userId, int offset, int limit);

  Task AddSongToLibrary(string userId, AddRemoveSongLibraryDTO addRemoveSong);

  Task AddAlbumToLibrary(string userId, AddRemoveAlbumLibraryDTO addRemoveAlbum);


  Task AddPlayListToLibrary(string userId, AddRemovePlayListDTO addRemovePlayListDTO);

  Task CheckSongsOnLibraryWithUser(ICollection<GetSongDTO> songsList, string userId);

}