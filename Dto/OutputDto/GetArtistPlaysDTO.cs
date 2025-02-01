namespace purpuraMain.Dto.OutputDto;

public class GetArtistPlaysDTO
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required string ImageUrl { get; set; }
  public required int Plays { get; set; }
}