namespace purpuraMain.Dto.OutputDto;

public class GerArtistDTO
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required string Description { get; set; }
  public required List<GetSongDTO> Songs { get; set; }
}