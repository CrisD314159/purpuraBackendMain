namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.Dto.OutputDto;
using purpuraMain.DbContext;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;

public class ArtistService(PurpuraDbContext dbContext, IMapper mapper, ILibraryService libraryService) : IArtistService
{

    private readonly PurpuraDbContext _dbContext = dbContext;

    private readonly ILibraryService _libraryService = libraryService;
    private readonly IMapper _mapper = mapper;
    /// <summary>
    /// Obtiene un artista por su ID.
    /// </summary>
    /// <param name="id">ID del artista.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetArtistDTO con la información del artista.</returns>
    public async Task<GetArtistDTO> GetArtistById(string userId, string artistId)
    {
        var artist = await _dbContext.Artists.Where(a => a.Id == artistId)
        .Include(a => a.Albums)
        .ProjectTo<GetArtistDTO>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Artist not found");

        artist.TopSongs = await GetTopArtistSongs(userId, artist.Id);

        return artist; 
    }

    /// <summary>
    /// Obtiene una lista de artistas que coincidan con el nombre ingresado.
    /// </summary>
    /// <param name="name">Nombre o parte del nombre del artista.</param>
    /// <param name="offset">Desplazamiento para la paginación.</param>
    /// <param name="limit">Límite de registros a devolver.</param>
    /// <param name="_dbContext">Contexto de base de datos.</param>
    /// <returns>Lista de artistas coincidentes.</returns>
    public async Task<List<GetArtistDTO>> GetArtistByName(string name, int offset, int limit)
    {
       
        var artists = await _dbContext.Artists.Where(a => a.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .Select(a => new GetArtistDTO
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                PictureUrl = a.PictureUrl
            }).OrderByDescending(a => a.Name)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return artists;
      
    }

    /// <summary>
    /// Obtiene los álbumes de un artista específico por su ID.
    /// </summary>
    /// <param name="id">ID del artista.</param>
    /// <param name="_dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetArtistDTO con la lista de álbumes del artista.</returns>
    public async Task<GetArtistDTO> GetArtistAlbums(string id)
    {
    
        var artist = await _dbContext.Artists.Where(a => a.Id == id)
        .ProjectTo<GetArtistDTO>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Artist not found");

        return artist;
    
    }

    /// <summary>
    /// Obtiene la lista de artistas más escuchados.
    /// </summary>
    /// <param name="offset">Desplazamiento para la paginación.</param>
    /// <param name="limit">Límite de registros a devolver.</param>
    /// <param name="_dbContext">Contexto de base de datos.</param>
    /// <returns>Lista de artistas más escuchados.</returns>
public async Task<List<GetArtistPlaysDTO>> GetMostListenArtists(int offset, int limit)
{
    var artistPlays = await _dbContext.Artists
        .Select(a => new
        {
            Artist = a,
            Plays = _dbContext.PlayHistories
                .Count(pl => pl.Song!.Artists!.Any(ar => ar.Id == a.Id))
        })
        .OrderByDescending(a => a.Plays)
        .Skip(offset)
        .Take(limit)
        .ToListAsync();

    // Mapear a DTO después de ejecutar la consulta
    var artists = artistPlays.Select(a => new GetArtistPlaysDTO
    {
        Id = a.Artist.Id,
        Name = a.Artist.Name,
        ImageUrl = a.Artist.PictureUrl ?? "",
        Plays = a.Plays
    }).ToList();

    return artists;
}

/// <summary>
/// Obtiene las canciones más escuchadas de un artista (No tiene endpoint dedicado).
/// </summary>
/// <param name="id"></param>
/// <param name="_dbContext"></param>
/// <returns></returns>
public async Task<List<GetSongDTO>> GetTopArtistSongs(string userId, string artistId)
{

    var artistSongs = await _dbContext.Songs.Where(s => s.Artists.Any(a => a.Id == artistId))
    .ProjectTo<GetSongDTO>(_mapper.ConfigurationProvider)
    .OrderByDescending(s => _dbContext.PlayHistories!.Count(ph => ph.SongId == s.Id))
    .Take(15)
    .ToListAsync();

    if(userId != "0") await _libraryService.CheckSongsOnLibraryWithUser(artistSongs, userId);


    return artistSongs;
    

}

}
