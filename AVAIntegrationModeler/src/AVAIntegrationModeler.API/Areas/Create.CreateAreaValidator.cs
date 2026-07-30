using FastEndpoints;
using FluentValidation;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Validátor pro požadavek na vytvoření oblasti.
/// </summary>
public class CreateAreaValidator : Validator<CreateAreaRequest>
{
  public CreateAreaValidator()
  {
    RuleFor(x => x.Area.Code)
      .NotEmpty()
      .WithMessage("Kód oblasti je povinný.")
      .MaximumLength(100);

    RuleFor(x => x.Area.Name)
      .NotEmpty()
      .WithMessage("Název oblasti je povinný.")
      .MaximumLength(200);
  }
}
