namespace purpuraMain.Dto.InputDto;

public class UpdateGenreDTO
{
  public required string Id {get; set;}
  public required string Name {get; set;}
  public string? Description {get; set;}
  public required string Color {get; set;}
  
}