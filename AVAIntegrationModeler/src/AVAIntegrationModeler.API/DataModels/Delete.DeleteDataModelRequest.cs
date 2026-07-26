using AVAIntegrationModeler.Contracts;
using FluentValidation;

namespace AVAIntegrationModeler.API.DataModels;

public class DeleteDataModelValidator : Validator<DeleteDataModelRequest>
{
  public DeleteDataModelValidator()
  {
    RuleFor(x => x.DataModelId).NotEmpty().WithMessage("DataModelId je povinný.");
  }
}
