using FastEndpoints;
using FluentValidation;
using static System.Net.WebRequestMethods;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Validator pro <see cref="DeleteScenarioRequest"/>
/// </summary>
/// <see href="https://fast-endpoints.com/docs/validation"/>
public class DeleteScenarioValidator : Validator<DeleteScenarioRequest>
{
  public DeleteScenarioValidator()
  {
    RuleFor(x => x.ScenarioId)
      .NotEmpty();
  }
}
