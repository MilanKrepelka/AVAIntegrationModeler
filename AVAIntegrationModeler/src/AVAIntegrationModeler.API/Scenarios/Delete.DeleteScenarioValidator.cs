using FastEndpoints;
using FluentValidation;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class DeleteScenarioValidator : Validator<DeleteScenarioRequest>
{
  public DeleteScenarioValidator()
  {
    RuleFor(x => x.ScenarioId)
      .NotEmpty();
  }
}
