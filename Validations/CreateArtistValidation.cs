using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;

public class CreateArtistValidation : AbstractValidator<CreateArtistDTO>
{
  public CreateArtistValidation()
  {
    RuleFor(artist => artist.Description).MinimumLength(5).MaximumLength(200);
    RuleFor(artist => artist.Name).MinimumLength(2).MaximumLength(80);
    RuleFor(artist => artist.PictureURL).NotEmpty().NotNull();
  }
}