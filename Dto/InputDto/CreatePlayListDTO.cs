using purpuraMain.Model;

namespace purpuraMain.Dto.InputDto;


public class CreatePlayListDTO
{
  public required string Name {get; set;}
  public string? ImageUrl {get; set;}
  public string? Description {get; set;}

}