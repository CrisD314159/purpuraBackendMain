using System.ComponentModel.DataAnnotations;
using purpuraMain.Model;

namespace purpuraMain.Dto.InputDto;

public class AddSongToAlbumDTO
{
  public required string Name { get; set; }
  public string? Disclaimer { get; set; }
  [Url]
  public required string AudioUrl { get; set; }
  public required string Lyrics { get; set; }
  public required Guid AlbumId { get; set; }
  public required List<Guid> Artists { get; set; }
  public required int AlbumTrack{ get; set; }
}