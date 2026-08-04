using FastEndpoints;
using FluentValidation;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// See: https://fast-endpoints.com/docs/validation
/// </summary>
public class GetScenariosByCodeValidator : Validator<GetScenarioByCodeRequest>
{
  public GetScenariosByCodeValidator()
  {
    RuleFor(x => x.ScenarioCode).NotEmpty();
  }
}
