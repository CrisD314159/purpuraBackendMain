using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;


public interface IAlbumService
{
    Task<GetAlbumDTO> GetAlbumById(string userId, Guid albumId);
    Task<List<GetAlbumDTO>> GetAlbumByInput(string input, int offset, int limit);
    Task<List<GetAlbumDTO>> GetAllAlbums(int offset, int limit);
    Task<List<GetAlbumDTO>> GetTopAlbums();
    Task CreateAlbum(CreateAlbumDTO createAlbumDTO);
    Task UpdateAlbum(UpdateAlbumDTO updateAlbumDTO);
    Task DeleteAlbum(Guid albumId);

}