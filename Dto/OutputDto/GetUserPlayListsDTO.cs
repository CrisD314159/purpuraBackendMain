namespace purpuraMain.Dto.OutputDto;

public class GetUserPlayListsDTO
{
  public required Guid Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public  string? UserId { get; set; }
  public  string? UserName { get; set; }
  public required bool IsPublic{ get; set; }
  public required string ImageUrl { get; set; }

}