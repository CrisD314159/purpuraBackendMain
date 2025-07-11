namespace purpuraMain.Services.Implementations;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using purpuraMain.Dto.InputDto;
using FluentValidation;
using purpuraMain.Model;

public class GenreService(PurpuraDbContext dbContext, IMapper mapper, ILibraryService libraryService,
IValidator<CreateGenreDTO> createGenreValidator, IValidator<UpdateGenreDTO> updateGenreValidator
) : IGenreService
{
    private readonly PurpuraDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ILibraryService _libraryService = libraryService;
    private readonly IValidator<CreateGenreDTO> _createGenreValidator = createGenreValidator;
    private readonly IValidator<UpdateGenreDTO> _updateGenreValidator = updateGenreValidator;
    /// <summary>
    /// Obtiene las canciones más populares de un género.
    /// </summary>
    /// <param name="id">ID del género.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetGenreDTO con la información del género y sus canciones más populares.</returns>
    public  async Task<GetGenreDTO> GetTopSongsByGenre(Guid genreId, string userId)
    {

        var genreInfoTopSongs = await _dbContext.Genres.Where(g => g.Id == genreId)
        .ProjectTo<GetGenreDTO>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Genre not found");

        // Obtener los id de las 10 canciones más reproducidas del género
        var topSongIds = await _dbContext.PlayHistories
            .Where(ph => ph.Song!.GenreId == genreId)
            .GroupBy(ph => ph.SongId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(15)
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
    
    /// <summary>
    /// Gets a genre using its id
    /// </summary>
    /// <param name="genreId"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
     public async Task<GetGenreDTO> GetGenreById(Guid genreId)
    {
        var genres = await _dbContext.Genres!.Where(g => g.Id == genreId).Select(g => new GetGenreDTO
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Color = g.Color
        }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Genre not found");

        return genres;
    }

    /// <summary>
    /// Creates a genre using a data transfer object
    /// </summary>
    /// <param name="createGenreDTO"></param>
    /// <returns></returns>
    /// <exception cref="BadHttpRequestException"></exception>
    public async Task CreateGenre(CreateGenreDTO createGenreDTO)
    {
        _createGenreValidator.ValidateAndThrow(createGenreDTO);

        if (await _dbContext.Genres.AnyAsync(g => g.Name == createGenreDTO.Name))
            throw new BadRequestException("Genre already exists");

        var genre = new Genre
        {
            Name = createGenreDTO.Name,
            Color = createGenreDTO.Color,
            Description = createGenreDTO.Description
        };

        await _dbContext.Genres.AddAsync(genre);
        await _dbContext.SaveChangesAsync();

    }

    /// <summary>
    /// Updates a Genre using a data transfer object
    /// </summary>
    /// <param name="updateGenreDTO"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task UpdateGenre(UpdateGenreDTO updateGenreDTO)
    {
        _updateGenreValidator.ValidateAndThrow(updateGenreDTO);

        var genre = await _dbContext.Genres.FindAsync(updateGenreDTO.Id)
        ?? throw new EntityNotFoundException("Genre not found");

        genre.Name = updateGenreDTO.Name;
        genre.Color = updateGenreDTO.Color;
        genre.Description = updateGenreDTO.Description;

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes permanently a genre using its ID
    /// </summary>
    /// <param name="genreId"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task DeleteGenre(Guid genreId)
    {
        var genre = await _dbContext.Genres.FindAsync(genreId)
        ?? throw new EntityNotFoundException("Genre not found");

        _dbContext.Genres.Remove(genre);
    }
}