using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.Scenarios.Create;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Create a new Scenario
/// </summary>
/// <remarks>
/// Creates a new Scenario given a datasource and scenario DTO.
/// </remarks>
public class CreateScenarioEndpoint(IMediator _mediator)
  : Endpoint<CreateScenarioRequest, CreateScenarioResponse>
{
  public override void Configure()
  {
    Post(CreateScenarioRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Vytvoří nový integrační scénář.";
      s.Description = "Vytvoří nový scénář v zadaném datasource. Vyžaduje validní ScenarioDTO.";
      s.ExampleRequest = new CreateScenarioRequest
      {
        Datasource = Contracts.Datasource.Database,
        Scenario = new Contracts.DTO.ScenarioDTO
        {
          Code = "TEST-001",
          Name = new Contracts.DTO.LocalizedValue
          {
            CzechValue = "Testovací scénář",
            EnglishValue = "Test Scenario"
          },
          Description = new Contracts.DTO.LocalizedValue
          {
            CzechValue = "Popis",
            EnglishValue = "Description"
          }
        }
      };
    });
  }

  public override async Task HandleAsync(
    CreateScenarioRequest request,
    CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new CreateScenarioCommand(request.Datasource, request.Scenario),
      cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateScenarioResponse(result.Value);
      
      await SendCreatedAtAsync<GetById>(
        new { datasource = request.Datasource, scenarioId = result.Value },
        Response,
        cancellation: cancellationToken);
      return;
    }

    // ✅ FastEndpoints způsob zpracování validačních chyb
    if (result.Status == ResultStatus.Invalid)
    {
      // Přidej každou chybu pomocí ThrowError nebo AddError
      foreach (var error in result.ValidationErrors)
      {
        AddError(error.Identifier ?? "Model", error.ErrorMessage);
      }
      
      // Pošle 400 s ValidationProblemDetails
      await SendErrorsAsync(400, cancellationToken);
      return;
    }

    // ✅ FastEndpoints způsob pro Conflict
    if (result.Status == ResultStatus.Conflict)
    {
      AddError(string.Join("; ", result.Errors));
      await SendErrorsAsync(409, cancellationToken);
      return;
    }

    // ✅ FastEndpoints způsob pro obecnou chybu
    AddError(string.Join("; ", result.Errors));
    await SendErrorsAsync(500, cancellationToken);
  }
}
