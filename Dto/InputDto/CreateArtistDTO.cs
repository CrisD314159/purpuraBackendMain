namespace purpuraMain.Dto.InputDto;


public class CreateArtistDTO
{
  public required string Name { get; set; }
  public string? Description { get; set; }
  public string? PictureURL { get; set; }
}