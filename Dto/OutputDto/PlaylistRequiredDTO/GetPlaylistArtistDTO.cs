namespace purpuraMain.Dto.OutputDto;

public class GetPlaylistArtistDTO
{
  public required Guid Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
}