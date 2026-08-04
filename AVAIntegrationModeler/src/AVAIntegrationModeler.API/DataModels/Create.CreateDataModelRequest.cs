using AVAIntegrationModeler.Contracts;
using FluentValidation;

namespace AVAIntegrationModeler.API.DataModels;

public class CreateDataModelValidator : Validator<CreateDataModelRequest>
{
  public CreateDataModelValidator()
  {
    RuleFor(x => x.DataModel).NotNull().WithMessage("DataModel je povinný.");
    RuleFor(x => x.DataModel.Code).NotEmpty().WithMessage("Kód je povinný.").MaximumLength(200);
    RuleFor(x => x.DataModel.Name).NotEmpty().WithMessage("Název je povinný.").MaximumLength(500);
  }
}
