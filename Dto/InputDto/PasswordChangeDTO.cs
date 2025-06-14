namespace purpuraMain.Dto.InputDto;

public class PasswordChangeDTO
{
    public required string Code { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}