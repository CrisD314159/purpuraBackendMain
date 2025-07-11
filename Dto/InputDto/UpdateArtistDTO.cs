namespace purpuraMain.Dto.InputDto;


public class UpdateArtistDTO
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public string? PictureURL { get; set; }
}