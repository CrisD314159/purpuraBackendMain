using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;

public class UpdateGenreValidation : AbstractValidator<UpdateGenreDTO>
{

  public UpdateGenreValidation()
  {
    RuleFor(g => g.Id).NotEmpty();
    RuleFor(g => g.Name).NotEmpty().MaximumLength(3).MaximumLength(20);
    RuleFor(g=> g.Color).NotEmpty().MaximumLength(4).MaximumLength(10);
    RuleFor(g=> g.Description).MaximumLength(50);
    
  }
  
}