using AVAIntegrationModeler.Contracts;
using FluentValidation;

namespace AVAIntegrationModeler.API.DataModels;

public class GetDataModelByIdValidator : Validator<GetDataModelRequest>
{
  public GetDataModelByIdValidator()
  {
    RuleFor(x => x.DataModelId).NotEmpty().WithMessage("DataModelId je povinný.");
  }
}
