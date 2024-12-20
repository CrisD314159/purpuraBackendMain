using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;


public class PlayListValidation : AbstractValidator<CreatePlayListDTO>
{
  public PlayListValidation()
  {
    RuleFor(p => p.Name).MinimumLength(2).MaximumLength(30).NotEmpty();
    RuleFor(p => p.UserId).NotEmpty();
    RuleFor(p => p.Description).MaximumLength(100);
  }
}