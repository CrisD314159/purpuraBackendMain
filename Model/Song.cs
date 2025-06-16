using System.ComponentModel.DataAnnotations;

namespace purpuraMain.Model;

public class Song
{
  public Guid Id { get; set; } = Guid.NewGuid();
  [MinLength(2)]
  public required string Name { get; set; }
  public required Guid AlbumId { get; set; }
  public required Album Album { get; set; }
  public string? Disclaimer { get; set; } = "";
  public string? Lyrics { get; set; } = "";
  public required string AudioUrl { get; set; }
  public required string ImageUrl { get; set; }
  public required DateTime DateAdded { get; set; }
  public ICollection<Playlist> Playlists { get; set; } = [];
  public ICollection<Artist> Artists { get; set; } = [];
  public required Guid GenreId { get; set; }
  public required Genre Genre { get; set; }
  public ICollection<Library> Libraries { get; set; } = [];

}