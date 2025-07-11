using System.ComponentModel.DataAnnotations;

namespace purpuraMain.Dto.InputDto;


public class CreateSingleSongDTO
{
  public required string Name { get; set; }
  public string? Disclaimer { get; set; }
  public string? Lyrics { get; set; }
  [Url]
  public required string ImageUrl { get; set; }
  [Url]
  public required string AudioUrl { get; set; }
  public required Guid GenreId { get; set; }
  public required List<Guid> Artists { get; set; }
}