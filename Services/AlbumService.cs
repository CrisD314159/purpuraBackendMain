namespace purpuraMain.Services;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;

public static class AlbumService
{
    public static async Task<GetAlbumDTO> GetAlbumById(string id, PurpuraDbContext dbContext)
    {
        try
        {
            var album = await dbContext.Albums!.Where(a=> a.Id == id).Select(a => new GetAlbumDTO
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
                Songs = dbContext.Songs != null ? dbContext.Songs.Where(s=> s.AlbumId == a.Id).Select(s=> new GetSongDTO
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
                    
                    }).ToList() : new List<GetSongDTO>(),

            }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Album not found");

            return album;
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
    public static async Task<List<GetAlbumDTO>> GetAlbumByInput(string input, PurpuraDbContext dbContext)
    {
          try
        {
            var album = await dbContext.Albums!.Where(a=> a.Name.ToLower().Contains(input.ToLower()) || a.Artist.Name.ToLower().Contains(input.ToLower())).Select(a => new GetAlbumDTO
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
                Writer = a.WriterName ?? ""

            }).ToListAsync() ?? throw new EntityNotFoundException("Album not found");

            return album;
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
    


}