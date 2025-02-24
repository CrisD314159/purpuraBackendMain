namespace purpuraMain.Dto.OutputDto;

public class GetLibraryPlaylistDTO{

  public required string Id { get; set; }
  public required string Name { get; set; }
  public  required string ImageUrl { get; set; }
  public string? Description { get; set; }
  public required string UserId{ get; set; }
  public required string UserName { get; set; }
  public required bool IsPublic{ get; set; }
}