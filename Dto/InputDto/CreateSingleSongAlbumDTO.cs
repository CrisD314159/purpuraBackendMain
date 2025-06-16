using System.ComponentModel.DataAnnotations;
using purpuraMain.Model;

namespace purpuraMain.Dto.InputDto;

public class CreateSingleSongAlbumDTO
{
  public required string Name { get; set; }
  public string? Disclaimer { get; set; }
  [Url]
  public required string ImageUrl { get; set; }
  public required Artist Artist { get; set; }
  public required Genre Genre{ get; set; }
}