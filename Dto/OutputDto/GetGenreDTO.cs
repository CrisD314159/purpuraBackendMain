using purpuraMain.Model;

namespace purpuraMain.Dto.OutputDto;

public class GetGenreDTO
{
  public required Guid Id { get; set; }
  public required string Name { get; set; }
  public string Color { get; set; } = "";
  public string? Description { get; set; } = "";

  public ICollection<GetSongDTO> Songs { get; set; } = [];

}