using AVAIntegrationModeler.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class CreateScenarioValidator : Validator<CreateScenarioRequest>
{
  public CreateScenarioValidator()
  {
    RuleFor(x => x.Scenario.Name.EnglishValue)
      .NotEmpty()
      .WithMessage("Name is required.")
      .MinimumLength(2)
      .MaximumLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);
  }
}
