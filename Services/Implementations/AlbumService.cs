namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using System.Threading.Tasks;

public class AlbumService(PurpuraDbContext dbContext, IMapper mapper, ILibraryService libraryService) : IAlbumService
{
    private readonly PurpuraDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;

    private readonly ILibraryService _libraryService = libraryService;


    /// <summary>
    /// Obtiene un álbum por su ID.
    /// </summary>
    /// <param name="id">ID del álbum.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetAlbumDTO con la información del álbum.</returns>
    /// <exception cref="EntityNotFoundException">Se lanza si el álbum no es encontrado.</exception>
    public async Task<GetAlbumDTO> GetAlbumById(string userId, string albumId)
    {

        var album = await _dbContext.Albums.Where(a => a.Id == albumId)
        .ProjectTo<GetAlbumDTO>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Album not found");
        
        if(userId != "0") await _libraryService.CheckSongsOnLibraryWithUser(album.Songs, userId);

        return album;

    }

 

    /// <summary>
    /// Busca álbumes por una cadena de entrada.
    /// </summary>
    /// <param name="input">Texto de búsqueda.</param>
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad máxima de elementos a devolver.</param>
    /// <param name="_dbContext">Contexto de base de datos.</param>
    /// <returns>Lista de álbumes que coinciden con la búsqueda.</returns>
    public async Task<List<GetAlbumDTO>> GetAlbumByInput(string input, int offset, int limit)
    {

        var albums = await _dbContext.Albums.Where(a => a.Name.Contains(input, StringComparison.OrdinalIgnoreCase) || a.Artist.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
        .ProjectTo<GetAlbumDTO>(_mapper.ConfigurationProvider)
        .Skip(offset)
        .Take(limit)
        .ToListAsync();

        return albums;

    }

    /// <summary>
    /// Obtiene todos los álbumes de la base de datos con paginación.
    /// </summary>
    /// <param name="offset">Número de elementos a omitir.</param>
    /// <param name="limit">Cantidad máxima de elementos a devolver.</param>
    /// <param name="_dbContext">Contexto de base de datos.</param>
    /// <returns>Lista de álbumes.</returns>
    public async Task<List<GetAlbumDTO>> GetAllAlbums(int offset, int limit)
    {

        var albums = await _dbContext.Albums
        .Where(a => a.AlbumType == 0)
        .ProjectTo<GetAlbumDTO>(_mapper.ConfigurationProvider)
        .OrderByDescending(a => a.Name).Skip(offset).Take(limit).ToListAsync();

        return albums;
      
    }

    /// <summary>
    /// Obtiene los álbumes más populares del momento.
    /// </summary>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<List<GetAlbumDTO>> GetTopAlbums()
    {
            var albums = await _dbContext.Albums.Where(a => a.AlbumType == 0).Select(a => new GetAlbumDTO
            {
                Id = a.Id,
                ArtistId = a.ArtistId,
                ArtistName = a.Artist.Name,
                Name = a.Name,
                PictureUrl = a.PictureUrl,
                Description = a.Description ?? "",
                TotalPlays = _dbContext.PlayHistories!.Where(s => s.Song!.AlbumId == a.Id).Count()
            }).OrderByDescending(a => a.TotalPlays).Take(10).ToListAsync();

            return albums;  
    }

}
