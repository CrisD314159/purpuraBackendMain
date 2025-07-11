namespace purpuraMain.Dto.InputDto;

public class VerifyAccountDTO
{
    public required string Email { get; set; }
    public required string Code { get; set; }
}