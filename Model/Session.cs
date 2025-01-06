namespace purpuraMain.Model;
public class Session
{
  public required string Id {get; set;}
  public required string UserId {get; set;}
  public required string FingerPrint {get; set;}

  public DateTime CreatedAt {get; set;}
}