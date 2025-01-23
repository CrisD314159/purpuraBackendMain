namespace purpuraMain.Model;
public class Session
{
  public required string Id {get; set;}
  public required string UserId {get; set;}
  public DateTime CreatedAt {get; set;}
  public DateTime ExpiresdAt {get; set;}
}