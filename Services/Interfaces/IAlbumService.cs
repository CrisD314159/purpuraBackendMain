using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;


public interface IAlbumService
{
    Task<GetAlbumDTO> GetAlbumById(string userId, string id);
    Task<List<GetAlbumDTO>> GetAlbumByInput(string input, int offset, int limit);
    Task<List<GetAlbumDTO>> GetAllAlbums(int offset, int limit);
    Task<List<GetAlbumDTO>> GetTopAlbums();

}