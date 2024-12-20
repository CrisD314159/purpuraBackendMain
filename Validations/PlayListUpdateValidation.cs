using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;


public class PlayListUpdateValidation : AbstractValidator<UpdatePlaylistDTO>
{
  public PlayListUpdateValidation()
  {
    RuleFor(p => p.Name).MinimumLength(2).MaximumLength(30).NotEmpty();
    RuleFor(p => p.Id).NotEmpty();
    RuleFor(p => p.Description).MaximumLength(100);
  }
}