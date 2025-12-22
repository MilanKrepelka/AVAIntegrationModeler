using System.Data.SqlTypes;
using Ardalis.Result;
using Ardalis.SharedKernel;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using AVAIntegrationModeler.UseCases.Scenarios.List;

namespace AVAIntegrationModeler.UseCases.Scenarios.Create;

public class CreateScenarioHandler(
  IRepository<Scenario> scenarioRepository,
  IIntegrationDataProvider integrationDataProvider,
  IListScenariosQueryService scenariosQueryService,
  IDomainEntityValidationService<Scenario> scenarioValidationService
) : ICommandHandler<CreateScenarioCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateScenarioCommand request, CancellationToken cancellationToken)
  {
    Scenario scenario;
    try
    {
      scenario = new Scenario(request.Scenario.Id);

      // Doménové Set* metody hází výjimky při porušení invariant
      scenario
        .SetCode(request.Scenario.Code)
        .SetName(new Domain.ValueObjects.LocalizedValue
        {
          CzechValue = request.Scenario.Description?.CzechValue ?? string.Empty,
          EnglishValue = request.Scenario.Description?.EnglishValue ?? string.Empty
        })
        .SetDescription(new Domain.ValueObjects.LocalizedValue
        {
          CzechValue = request.Scenario.Description?.CzechValue ?? string.Empty,
          EnglishValue = request.Scenario.Description?.EnglishValue ?? string.Empty
        })
        .SetInputFeature(request.Scenario.InputFeatureId)
        .SetOutputFeature(request.Scenario.OutputFeatureId);
    }
    catch (ArgumentOutOfRangeException ex)
    {
      return Result<Guid>.Invalid(new ValidationError
      {
        Identifier = ex.ParamName ?? "Scenario",
        ErrorMessage = ex.Message
      });
    }

    catch (ArgumentException ex)
    {
      // ✅ Převod doménové výjimky na Result.Invalid
      return Result<Guid>.Invalid(new ValidationError
      {
        Identifier = ex.ParamName ?? "Scenario",
        ErrorMessage = ex.Message
      });
    }

    catch (Exception ex)
    {
      // ✅ Převod doménové výjimky na Result.Invalid
      return Result<Guid>.Invalid(new ValidationError
      {
        Identifier = "Scenario",
        ErrorMessage = ex.Message
      });
    }


    // 2. Cross-aggregate/async validace (unikátnost Code/Id, existence Feature)
    var validationResult = await scenarioValidationService.ValidateForCreate(
      request.Datasource,
      scenario,
      cancellationToken);

    if (!validationResult.IsSuccess)
    {
      // ✅ Předání validačních chyb ze služby
      return Result<Guid>.Invalid(validationResult.ValidationErrors);
    }

    if (request.Datasource == Datasource.Database)
    {
      // 3. Uložení do repository
      Scenario? createdScenario = null;
      try
      {
        createdScenario = await scenarioRepository.AddAsync(scenario, cancellationToken);
        scenariosQueryService.InvalidateCache(request.Datasource);
      }
      catch( Exception ex)
      {
        return Result<Guid>.Error($"Scénář se nepodařilo vytvořit: {ex.Message}");
      }
      if (createdScenario is null)
      {
        return Result<Guid>.Error("Scénář nebyl vytvořen.");
      }
      return Result<Guid>.Success(createdScenario.Id);
    }
    else if (request.Datasource == Datasource.AVAPlace)
    {
      // TODO
      var created = await integrationDataProvider.CreateScenario(request.Scenario, cancellationToken);
      scenariosQueryService.InvalidateCache(request.Datasource);
      return created ? Result.Success() : Result.Error("Scénář nebyl vytvořen.");
    }
    return Result.Error("Scénář nebyl vytvořen.");
  }
}


