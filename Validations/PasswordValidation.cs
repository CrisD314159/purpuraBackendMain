namespace purpuraMain.Validations;
using FluentValidation;
using purpuraMain.Dto.InputDto;

public class PasswordValidation : AbstractValidator<PasswordChangeDTO>
{
    public PasswordValidation()
    {
        RuleFor(user => user.Code).NotEmpty();
        RuleFor(user => user.Email).EmailAddress().NotEmpty();
        RuleFor(user => user.Password).MinimumLength(8).MaximumLength(20).NotEmpty().Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\-])[A-Za-z\d@$!%*?&\-]{8,}$")
        .WithMessage("Password must be at least 8 characters long, include at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*-?&).");;
    }
}