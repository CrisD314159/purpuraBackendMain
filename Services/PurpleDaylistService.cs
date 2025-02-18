using Microsoft.EntityFrameworkCore;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Model;

namespace purpuraMain.Services;
public static class PurpleDaylistService
{



  public static async Task<GetPlayListDTO> GetPurpleDaylist(string userId, PurpuraDbContext dbContext)
  {
    try
    {
      var playlist = await dbContext.Playlists!.Where(p=> p.Name == "Purple Day List" && p.UserId == userId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");
      await UpdatePurpuraDayList(playlist.Id, userId, dbContext);
      var entirePlaylist = await dbContext.Playlists!.Where(p => p.Id == playlist.Id).Select(p => new GetPlayListDTO
      {
                Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        UserName = p.User!.FirstName!,
        IsPublic = p.IsPublic,
        ImageUrl = p.ImageUrl,
        Songs = p.Songs != null ? p.Songs.Select(s=> new GetSongDTO
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
          IsOnLibrary = false

        }).ToList() : new List<GetSongDTO>()
      }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");

        foreach (var song in entirePlaylist.Songs)
        {
          song.IsOnLibrary = await dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).AnyAsync(l => l.Songs.Any(so => so.Id == song.Id));
        }

      return entirePlaylist;

    }
    catch (System.Exception)
    {
      throw;
    }
  }

  public static async Task<bool> UpdatePurpuraDayList(string playlistId, string userId, PurpuraDbContext dbContext)
  {
    try
    {
      var playlist = dbContext.Playlists!.Find(playlistId) ?? throw new EntityNotFoundException("Playlist not found");
      if((DateTime.UtcNow.Date - playlist.LastUpdated.Date ) > TimeSpan.FromDays(1))
      {
        playlist.LastUpdated = DateTime.UtcNow;
        var recomendations =  await GetUserRecomendations(userId, dbContext);
        playlist.Songs =recomendations;
        await dbContext.SaveChangesAsync();
        return true;
      }
      return false;
    }
    catch (System.Exception e)
    {

      Console.WriteLine(e.Message);
      
      throw;
    }
  }
 public static async Task<List<Song>> GetUserRecomendations(string userId, PurpuraDbContext dbContext)
{
    try
    {
        // Obtener las canciones reproducidas recientemente y sus géneros
        var recentListenGenres = await dbContext.PlayHistories!
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PlayedAt)
            .Take(10)
            .SelectMany(p => p.Song!.Genres!)  // Aplanamos la lista
            .Select(g => g.Id) // Tomamos solo los IDs de los géneros
            .Distinct()
            .ToListAsync();

        // Buscar canciones que tengan al menos un género en la lista obtenida
        var recomendations = await dbContext.Songs!
            .Where(s => s.Genres!.Any(g => recentListenGenres.Contains(g.Id))) // Comparamos con IDs
            .OrderByDescending(s => s.Name)
            .Take(10)
            .ToListAsync();

        return recomendations;
    }
    catch (System.Exception e)
    {
        Console.WriteLine(e.Message);
        throw;
    }
}

}