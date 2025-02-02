namespace purpuraMain.Services;
using purpuraMain.Model;
using purpuraMain.Dto.OutputDto;
using purpuraMain.DbContext;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;

public static class ArtistService
{
    /// <summary>
    /// Obtiene un artista por su ID.
    /// </summary>
    /// <param name="id">ID del artista.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetArtistDTO con la información del artista.</returns>
    public static async Task<GetArtistDTO> GetArtistById(string id, PurpuraDbContext dbContext)
    {
        try
        {
            var artist = await dbContext.Artists!.Where(a=> a.Id == id).Select(a => new GetArtistDTO
            {
                 Id = a.Id,
                 Name = a.Name,
                 Description = a.Description ?? "",
                 ImageUrl = a.PictureUrl ?? ""
            }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Artist not found");

            return artist;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Obtiene una lista de artistas que coincidan con el nombre ingresado.
    /// </summary>
    /// <param name="name">Nombre o parte del nombre del artista.</param>
    /// <param name="offset">Desplazamiento para la paginación.</param>
    /// <param name="limit">Límite de registros a devolver.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Lista de artistas coincidentes.</returns>
    public static async Task<List<GetArtistDTO>> GetArtistByName(string name, int offset, int limit, PurpuraDbContext dbContext)
    {
        try
        {
            var artists = await dbContext.Artists!.Where(a => a.Name.ToLower().Contains(name.ToLower()))
                .Select(a => new GetArtistDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description ?? "",
                    ImageUrl = a.PictureUrl ?? ""
                }).Skip(offset).Take(limit).ToListAsync() ?? [];

            return artists;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Obtiene los álbumes de un artista específico por su ID.
    /// </summary>
    /// <param name="id">ID del artista.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Objeto GetArtistDTO con la lista de álbumes del artista.</returns>
    public static async Task<GetArtistDTO> GetArtistAlbums(string id, PurpuraDbContext dbContext)
    {
        try
        {
            var artist = await dbContext.Artists!.Where(a=> a.Id == id).Select(a => new GetArtistDTO
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description ?? "",
                ImageUrl = a.PictureUrl ?? "",
                Albums = dbContext.Albums != null ? dbContext.Albums.Where(al => al.ArtistId == a.Id)
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
        catch (System.Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Obtiene la lista de artistas más escuchados.
    /// </summary>
    /// <param name="offset">Desplazamiento para la paginación.</param>
    /// <param name="limit">Límite de registros a devolver.</param>
    /// <param name="dbContext">Contexto de base de datos.</param>
    /// <returns>Lista de artistas más escuchados.</returns>
    public static async Task<List<GetArtistPlaysDTO>> GetMostListenArtists(int offset, int limit, PurpuraDbContext dbContext)
    {
        try
        {
            var artists = await dbContext.Artists!.Select(a => new GetArtistPlaysDTO
            {
                Id = a.Id,
                Name = a.Name,
                ImageUrl = a.PictureUrl ?? "",
                Plays = dbContext.PlayHistories!.Where(pl => pl.Song!.Artists != null && pl.Song.Artists.Any(ar => ar.Id == a.Id)).Count()
            }).Skip(offset).Take(limit).OrderByDescending(a => a.Plays).ToListAsync()
            ?? throw new EntityNotFoundException("Not found artists");

            return artists;     
        }
        catch (System.Exception)
        {
            throw;
        }
    }
}
