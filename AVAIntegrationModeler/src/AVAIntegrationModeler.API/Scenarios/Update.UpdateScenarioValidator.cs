using AVAIntegrationModeler.Infrastructure.Data.Config;
using FastEndpoints;
using FluentValidation;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class UpdateScenarioValidator : Validator<UpdateScenarioRequest>
{
  public UpdateScenarioValidator()
  {
    RuleFor(x => x.Scenario)
      .NotNull()
      .WithMessage("Scenario is required.");
  }
}
