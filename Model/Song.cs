using System.ComponentModel.DataAnnotations;

namespace purpuraMain.Model;

public class Song
{
  public required string Id { get; set; }

  [MinLength(2)]
  public required string Name { get; set; }

  public required string AlbumId { get; set; }
  public required Album Album { get; set; }

  public string Lyrics { get; set; } = "";
  public required string AudioUrl { get; set; }
  public required string ImageUrl { get; set; }

  public ICollection<Playlist> Playlists { get; set; } =[];

  public ICollection<Artist> Artists { get; set; } =[];

  public ICollection<Genre> Genres { get; set; } =[];

  public ICollection<Library> Libraries { get; set; }=[];
}