namespace purpuraMain.Services;
using purpuraMain.Model;
using purpuraMain.Dto.OutputDto;
using purpuraMain.DbContext;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;

public static class ArtistService
{
    public static async Task<GetArtistDTO> GetArtistById(string id, PurpuraDbContext dbContext)
    {
        try
        {
            var artist = await dbContext.Artists!.Where(a=> a.Id == id).Select(a => new GetArtistDTO
            {
                 Id = a.Id,
                 Name = a.Name,
                 Description = a.Description ?? "",
                 TopSongs = a.Songs != null ? a.Songs.Select(s=> new GetSongDTO
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
                    Plays = dbContext.PlayHistories!.Where(pl => pl.SongId == s.Id).Count()
                    
                    }).OrderBy(a=> a.Plays).ToList() : new List<GetSongDTO>(),
                    Albums = dbContext.Albums != null ? dbContext.Albums.Where(al => al.ArtistId == a.Id).Select(al => new GetLibraryAlbumDTO{
                        ArtistId = al.ArtistId,
                        ArtistName = a.Name,
                        Id = al.Id,
                        Name = al.Name,
                        PictureUrl = al.PictureUrl,
                        Description = al.Description ?? "",
                        ReleaseDate = al.ReleaseDate
                    }).Take(10).ToList() : new List<GetLibraryAlbumDTO>(),
                    ImageUrl = a.PictureUrl ?? ""
                            
                }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Artist not found");

            return artist;
        }
        catch(EntityNotFoundException)
        {
            throw;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    public static async Task<List<GetArtistDTO>> GetArtistByName(string name,int offset, int limit, PurpuraDbContext dbContext)
    {

        try
        {
            var artists = await dbContext.Artists!.Where(a => a.Name.ToLower().Contains(name.ToLower())).Select(a=> new GetArtistDTO
            {
                 Id = a.Id,
                 Name = a.Name,
                 Description = a.Description ?? "",
                 ImageUrl = a.PictureUrl ?? ""
            }).Skip(offset).Take(limit).ToListAsync() ?? [];

            return artists;
            
        }
         catch(EntityNotFoundException)
        {
            throw;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    public static async Task<GetArtistDTO> GetArtistAlbums(string id, PurpuraDbContext dbContext)
    {
        try
        {
            var artist = await dbContext.Artists!.Where(a=> a.Id == id).Select(a => new GetArtistDTO
            {
                 Id = a.Id,
                 Name = a.Name,
                 Description = a.Description ?? "",
                Albums = dbContext.Albums != null ? dbContext.Albums.Where(al => al.ArtistId == a.Id).Select(al => new GetLibraryAlbumDTO{
                        ArtistId = al.ArtistId,
                        ArtistName = a.Name,
                        Id = al.Id,
                        Name = al.Name,
                        PictureUrl = al.PictureUrl,
                        Description = al.Description ?? "",
                        ReleaseDate = al.ReleaseDate
                }).Take(10).ToList() : new List<GetLibraryAlbumDTO>(),
                ImageUrl = a.PictureUrl ?? ""
                            
                }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Artist not found");

            return artist;
            
        }
        catch(EntityNotFoundException)
        {
            throw;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }
    public static async Task<GetArtistDTO> GetArtistSongs(string id, PurpuraDbContext dbContext)
    {
        try
        {
            var artist = await dbContext.Artists!.Where(a=> a.Id == id).Select(a => new GetArtistDTO
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description ?? "",
                TopSongs = a.Songs != null ? a.Songs.Select(s=> new GetSongDTO
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
                    Plays = dbContext.PlayHistories!.Where(pl => pl.SongId == s.Id).Count()
                    
                    }).OrderBy(a=> a.Plays).ToList() : new List<GetSongDTO>(),
                ImageUrl = a.PictureUrl ?? ""
                            
                }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Artist not found");

            return artist;
            
        }
        catch(EntityNotFoundException)
        {
            throw;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }


    public static async Task<List<GetArtistDTO>> GetArtists(int offset, int limit, PurpuraDbContext dbContext)
    {
        try
        {
            var artists = await dbContext.Artists!.Select(a => new GetArtistDTO{
                Id = a.Id,
                Name = a.Name,
                Description = a.Description ?? "",
                ImageUrl = a.PictureUrl ?? ""
            }).Skip(offset).Take(limit).ToListAsync() ?? [];

            return artists;
        }
        catch(EntityNotFoundException)
        {
            throw;
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }
// Rest of the crud methods will be implemented in a nodejs standalone api

}