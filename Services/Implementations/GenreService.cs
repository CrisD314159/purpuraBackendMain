namespace purpuraMain.Services.Implementations;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;

public class GenreService(PurpuraDbContext dbContext) : IGenreService
{

    private readonly PurpuraDbContext _dbContext = dbContext;
    /// <summary>
    /// Obtiene las canciones más populares de un género.
    /// </summary>
    /// <param name="id">ID del género.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetGenreDTO con la información del género y sus canciones más populares.</returns>
    public  async Task<GetGenreDTO> GetTopSongsByGenre(string id)
    {
      
            var songs = await _dbContext.Genres!.Where(g => g.Id == id).Select(g=> new GetGenreDTO
            {
                 Id = g.Id,
                Name = g.Name,
                Description = g.Description ?? "",
                Color = g.Color,
                Songs = g.Songs.Select(
                    s=> new GetSongDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Artists = s.Artists != null ? s.Artists.Select(a => new GetPlaylistArtistDTO
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Description = a.Description??""
                        }).ToList() : new List<GetPlaylistArtistDTO>(),
                        AlbumId = s.AlbumId!,
                        AlbumName = s.Album!.Name!,
                        Duration = s.Duration,
                        ImageUrl = s.ImageUrl?? "",
                        AudioUrl = s.AudioUrl?? "",
                        Genres = s.Genres!.Select(g => new GetGenreDTO
                        {
                            Id = g.Id,
                            Name = g.Name,
                            Description = g.Description?? ""
                        }).ToList(),
                        Lyrics = s.Lyrics ?? "",
                        Plays = _dbContext.PlayHistories!.Where(pl => pl.SongId == s.Id).Count()
                        
                    }).OrderBy(s => s.Plays).ToList()
            }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message ="Genre not found", Success=false});
            
            return songs;

        
    }

    /// <summary>
    /// Obtiene todos los géneros disponibles en la plataforma.
    /// </summary>
    /// <param name="_dbContext">Contexto de base de datos.</param>
    /// <returns>Lista de objetos GetGenreDTO con la información de los géneros.</returns>
     public  async Task<List<GetGenreDTO>> GetAllGenres()
    {

      
            var genres = await _dbContext.Genres!.Select(g => new GetGenreDTO
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description ?? "",
                Color = g.Color
            }).ToListAsync() ?? [];

            return genres;

        
    }
    
     public  async Task<GetGenreDTO> GetGenreById(string id)
    {

      
            var genres = await _dbContext.Genres!.Where(g => g.Id == id).Select(g => new GetGenreDTO
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description ?? "",
                Color = g.Color
            }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message ="Genre not found", Success=false});

            return genres;

        
    }
}