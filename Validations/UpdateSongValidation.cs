using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;

public class UpdateSongValidation : AbstractValidator<UpdateSingleSongDTO>
{
  public UpdateSongValidation()
  {
    RuleFor(song => song.Disclaimer).MinimumLength(5).MaximumLength(100);
    RuleFor(song => song.Name).MinimumLength(2).MaximumLength(80);
  }
}