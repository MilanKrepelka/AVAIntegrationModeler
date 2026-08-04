using AVAIntegrationModeler.Contracts;
using FluentValidation;

namespace AVAIntegrationModeler.API.DataModels;

public class ImportDataModelFromAvaPlaceValidator : Validator<ImportDataModelFromAvaPlaceRequest>
{
  public ImportDataModelFromAvaPlaceValidator()
  {
    RuleFor(x => x.AvaPlaceModelId)
      .NotEmpty()
      .WithMessage("AvaPlaceModelId je povinný.");
  }
}
