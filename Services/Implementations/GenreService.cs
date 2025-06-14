namespace purpuraMain.Services.Implementations;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;
using AutoMapper.QueryableExtensions;
using AutoMapper;

public class GenreService(PurpuraDbContext dbContext, IMapper mapper, ILibraryService libraryService) : IGenreService
{
    private readonly PurpuraDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ILibraryService _libraryService = libraryService;
    /// <summary>
    /// Obtiene las canciones más populares de un género.
    /// </summary>
    /// <param name="id">ID del género.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetGenreDTO con la información del género y sus canciones más populares.</returns>
    public  async Task<GetGenreDTO> GetTopSongsByGenre(string genreId, string userId)
    {

        var genreInfoTopSongs = await _dbContext.Genres.Where(g => g.Id == genreId)
        .ProjectTo<GetGenreDTO>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Genre not found");

        // Obtener los id de las 10 canciones más reproducidas del género
        var topSongIds = await _dbContext.PlayHistories
            .Where(ph => ph.Song!.Genres.Any(g => g.Id == genreId))
            .GroupBy(ph => ph.SongId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(10)
            .ToListAsync();

        //Traer los datos completos de esas canciones
        var topSongs = await _dbContext.Songs
            .Where(s => topSongIds.Contains(s.Id))
            .ProjectTo<GetSongDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if(userId != "0") await _libraryService.CheckSongsOnLibraryWithUser(topSongs, userId);

        genreInfoTopSongs.Songs = topSongs;
            
        return genreInfoTopSongs;
 
    }

    /// <summary>
    /// Obtiene todos los géneros disponibles en la plataforma.
    /// </summary>
    /// <param name="_dbContext">Contexto de base de datos.</param>
    /// <returns>Lista de objetos GetGenreDTO con la información de los géneros.</returns>
     public  async Task<List<GetGenreDTO>> GetAllGenres()
    {

    
        var genres = await _dbContext.Genres.Select(g => new GetGenreDTO
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description ?? "",
            Color = g.Color
        }).ToListAsync();

        return genres;

        
    }
    
     public  async Task<GetGenreDTO> GetGenreById(string id)
    {
        var genres = await _dbContext.Genres!.Where(g => g.Id == id).Select(g => new GetGenreDTO
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Color = g.Color
        }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Genre not found");

        return genres;
    }
}