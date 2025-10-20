using FastEndpoints;
using FluentValidation;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class GetScenariosValidator : Validator<GetScenarioByIdRequest>
{
  public GetScenariosValidator()
  {
    RuleFor(x => x.ScenarioId).NotEmpty();
  }
}
