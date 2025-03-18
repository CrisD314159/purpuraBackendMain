namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;

public class AlbumService(PurpuraDbContext dbContext) : IAlbumService
{
    private readonly PurpuraDbContext _dbContext = dbContext;


    /// <summary>
    /// Obtiene un álbum por su ID.
    /// </summary>
    /// <param name="id">ID del álbum.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetAlbumDTO con la información del álbum.</returns>
    /// <exception cref="EntityNotFoundException">Se lanza si el álbum no es encontrado.</exception>
    public async Task<GetAlbumDTO> GetAlbumById(string userId, string id)
    {
       
            var album = await _dbContext.Albums!.Where(a => a.Id == id).Select(a => new GetAlbumDTO
            {
                ArtistId = a.ArtistId,
                ArtistName = a.Artist.Name,
                Id = a.Id,
                Name = a.Name,
                PictureUrl = a.PictureUrl,
                Description = a.Description ?? "",
                ReleaseDate = a.ReleaseDate,
                Producer = a.ProducerName ?? "",
                RecordLabel = a.RecordLabel ?? "",
                Writer = a.WriterName ?? "",
                GenreId = a.GenreId,
                GenreName = a.Genre!.Name,
                AlbumType = a.AlbumType,
                Songs = _dbContext.Songs != null ? _dbContext.Songs.Where(s => s.AlbumId == a.Id).Select(s => new GetSongDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Artists = s.Artists != null ? s.Artists.Select(a => new GetPlaylistArtistDTO
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description ?? ""
                    }).ToList() : new List<GetPlaylistArtistDTO>(),
                    AlbumId = s.AlbumId!,
                    AlbumName = s.Album!.Name!,
                    Duration = s.Duration,
                    ImageUrl = s.ImageUrl ?? "",
                    AudioUrl = s.AudioUrl ?? "",
                    Genres = s.Genres!.Select(g => new GetGenreDTO
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description ?? ""
                    }).ToList(),
                    Lyrics = s.Lyrics ?? "",
                    IsOnLibrary = false
                }).ToList() : new List<GetSongDTO>(),

            }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message ="Album not found", Success=false});


            if(album != null && album.Songs != null)
            {

                foreach (var song in album.Songs)
                {
                    song.IsOnLibrary = _dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).Any(l => l.Songs.Any(so => so.Id == song.Id));
                }
            }

            return album ?? throw new EntityNotFoundException(404, new {Message ="Album not found", Success=false});
      
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
       
            var album = await _dbContext.Albums!.Where(a => a.Name.ToLower().Contains(input.ToLower()) || a.Artist.Name.ToLower().Contains(input.ToLower())).Select(a => new GetAlbumDTO
            {
                ArtistId = a.ArtistId,
                ArtistName = a.Artist.Name,
                Id = a.Id,
                Name = a.Name,
                PictureUrl = a.PictureUrl,
                Description = a.Description ?? "",
                ReleaseDate = a.ReleaseDate,
                Producer = a.ProducerName ?? "",
                RecordLabel = a.RecordLabel ?? "",
                Writer = a.WriterName ?? "",
                GenreId = a.GenreId,
                GenreName = a.Genre!.Name,
                AlbumType = a.AlbumType,
            }).Skip(offset).Take(limit).ToListAsync() ?? [];

            return album;
      
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
       
            var albums = await _dbContext.Albums!.Where(a => a.AlbumType == 0).Select(a => new GetAlbumDTO
            {
                ArtistId = a.ArtistId,
                ArtistName = a.Artist.Name,
                Id = a.Id,
                Name = a.Name,
                PictureUrl = a.PictureUrl,
                Description = a.Description ?? "",
                ReleaseDate = a.ReleaseDate,
                Producer = a.ProducerName ?? "",
                RecordLabel = a.RecordLabel ?? "",
                Writer = a.WriterName ?? "",
                GenreId = a.GenreId,
                GenreName = a.Genre!.Name,
                AlbumType = a.AlbumType,
                Songs = _dbContext.Songs != null ? _dbContext.Songs.Where(s => s.AlbumId == a.Id).Select(s => new GetSongDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Artists = s.Artists != null ? s.Artists.Select(a => new GetPlaylistArtistDTO
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description ?? ""
                    }).ToList() : new List<GetPlaylistArtistDTO>(),
                    AlbumId = s.AlbumId!,
                    AlbumName = s.Album!.Name!,
                    Duration = s.Duration,
                    ImageUrl = s.ImageUrl ?? "",
                    AudioUrl = s.AudioUrl ?? "",
                    Genres = s.Genres!.Select(g => new GetGenreDTO
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description ?? ""
                    }).ToList(),
                    Lyrics = s.Lyrics ?? "",
                }).ToList() : new List<GetSongDTO>()
            }).OrderByDescending(a=> a.ReleaseDate).Skip(offset).Take(limit).ToListAsync() ?? [];

            return albums;
      
    }

    /// <summary>
    /// Obtiene los álbumes más populares del momento.
    /// </summary>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<List<GetAlbumDTO>> GetTopAlbums()
    {
       
            var albums = await _dbContext.Albums!.Where(a => a.AlbumType == 0).Select(a => new GetAlbumDTO
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
