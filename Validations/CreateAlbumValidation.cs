using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;


public class CreateAlbumValidation : AbstractValidator<CreateAlbumDTO>
{
  public CreateAlbumValidation()
  {
    RuleFor(album => album.Name).MinimumLength(2).MaximumLength(80);
    RuleFor(album => album.Disclaimer).MinimumLength(5).MaximumLength(200);
    RuleFor(album => album.Description).MinimumLength(5).MaximumLength(200);
    RuleFor(album => album.GenreId).NotEmpty().NotNull();
    RuleFor(album => album.ImageUrl).NotEmpty().NotNull();

  }
}