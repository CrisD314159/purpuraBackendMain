namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.Dto.OutputDto;
using purpuraMain.DbContext;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;

public class ArtistService(PurpuraDbContext dbContext) : IArtistService
{

    private readonly PurpuraDbContext _dbContext = dbContext;
    /// <summary>
    /// Obtiene un artista por su ID.
    /// </summary>
    /// <param name="id">ID del artista.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetArtistDTO con la información del artista.</returns>
    public async Task<GetArtistDTO> GetArtistById(string userId, string id)
    {
       
            var artistEntity = await _dbContext.Artists!.Where(a => a.Id == id).FirstOrDefaultAsync() ?? 
            throw new EntityNotFoundException("Artist not found");
            
            var artist = new GetArtistDTO
            {
                Id = artistEntity.Id,
                Name = artistEntity.Name,
                Description = artistEntity.Description ?? "",
                ImageUrl = artistEntity.PictureUrl ?? "",
                TopSongs = await GetTopArtistSongs(userId, artistEntity.Id),
                Albums = _dbContext.Albums != null ? _dbContext.Albums.Where(al => al.ArtistId == artistEntity.Id)
                    .Where(a => a.AlbumType == 0)
                    .Select(al => new GetLibraryAlbumDTO
                    {
                        ArtistId = al.ArtistId,
                        ArtistName = artistEntity.Name,
                        Id = al.Id,
                        Name = al.Name,
                        PictureUrl = al.PictureUrl,
                        Description = al.Description ?? "",
                        ReleaseDate = al.ReleaseDate
                    })
                    .OrderByDescending(al => al.ReleaseDate)
                    .Take(10).ToList() : []
            };


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
       
            var artists = await _dbContext.Artists!.Where(a => a.Name.ToLower().Contains(name.ToLower()))
                .Select(a => new GetArtistDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description ?? "",
                    ImageUrl = a.PictureUrl ?? ""
                }).OrderByDescending(a=> a.Name).Skip(offset).Take(limit).ToListAsync() ?? [];

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
       
            var artist = await _dbContext.Artists!.Where(a=> a.Id == id).Select(a => new GetArtistDTO
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description ?? "",
                ImageUrl = a.PictureUrl ?? "",
                Albums = _dbContext.Albums != null ? _dbContext.Albums.Where(al => al.ArtistId == a.Id)
                    .Select(al => new GetLibraryAlbumDTO
                    {
                        ArtistId = al.ArtistId,
                        ArtistName = a.Name,
                        Id = al.Id,
                        Name = al.Name,
                        PictureUrl = al.PictureUrl,
                        Description = al.Description ?? "",
                        ReleaseDate = al.ReleaseDate
                    }).Take(10).ToList() : new List<GetLibraryAlbumDTO>()
            }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Artist not found");

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
    try
    {
        var artistPlays = await _dbContext.Artists!
            .Select(a => new
            {
                Artist = a,
                Plays = _dbContext.PlayHistories!
                    .Count(pl => pl.Song!.Artists!.Any(ar => ar.Id == a.Id))
            })
            .OrderByDescending(a => a.Plays) // Ordenamos antes de paginar
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
    catch (System.Exception)
    {
        throw;
    }
}

/// <summary>
/// Obtiene las canciones más escuchadas de un artista (No tiene endpoint dedicado).
/// </summary>
/// <param name="id"></param>
/// <param name="_dbContext"></param>
/// <returns></returns>
public async Task<List<GetSongDTO>> GetTopArtistSongs(string userId, string id)
{
    try
    {
        var songsPlays = await _dbContext.Songs!
            .Where(s => s.Artists!.Any(art => art.Id == id))
            .Select(s => new GetSongDTO
            {
                Id = s.Id,
                Name = s.Name,
                Artists = s.Artists!.Select(a => new GetPlaylistArtistDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description ?? ""
                }).ToList(),
                AlbumId = s.AlbumId ?? "",
                AlbumName = s.Album!.Name ?? "",
                ImageUrl = s.ImageUrl ?? "",
                AudioUrl = s.AudioUrl ?? "",
                IsOnLibrary = false,
                Plays = _dbContext.PlayHistories!.Count(ph => ph.SongId == s.Id)
            })
            .OrderByDescending(s => _dbContext.PlayHistories!.Count(ph => ph.SongId == s.Id))
            .Take(15)
            .ToListAsync();

        foreach (var song in songsPlays)
        {
            song.IsOnLibrary = _dbContext.Libraries!.Where (l => l.UserId == userId && l.User!.State == UserState.ACTIVE).Any(l => l.Songs.Any(so=> so.Id == song.Id));
            
        }

        return songsPlays;
    }
    catch (System.Exception)
    {
        throw;
    }
}

}
