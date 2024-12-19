namespace purpuraMain.Dto.InputDto;

public class PasswordChangeDTO
{
    public int Code { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}