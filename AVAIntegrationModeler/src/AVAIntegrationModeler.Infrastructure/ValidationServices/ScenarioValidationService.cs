using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.UseCases.Scenarios;

namespace AVAIntegrationModeler.Infrastructure.ValidationServices;

/// <inheritdoc/>
public class ScenarioValidationService : IDomainEntityValidationService<Scenario>
{
  private readonly IScenariosQueryService _listScenariosQueryService;

  public ScenarioValidationService(IScenariosQueryService listScenariosQueryService)
  {
    _listScenariosQueryService = listScenariosQueryService;
  }

  /// <inheritdoc/>
  public async Task<Result> Validate(Datasource datasource, Scenario domainEntity, CancellationToken ct)
  {
    var code = domainEntity.Code?.Trim() ?? string.Empty;
    if (string.IsNullOrEmpty(code))
      return Result.Success();

    if (!await _listScenariosQueryService.ExistsByCodeAsync(datasource, code, ct))
      return Result.Success();

    try
    {
      var existing = await _listScenariosQueryService.GetScenario(datasource, code, ct);
      if (existing.Id != domainEntity.Id)
      {
        return Result.Invalid(new ValidationError(nameof(Scenario.Code),
          $"Kód scénáře '{code}' musí být unikátní v datasource {datasource}."));
      }
    }
    catch (NotFoundException)
    {
      // Mezitím byl scénář se stejným kódem smazán — kolize již neplatí.
    }

    return Result.Success();
  }

  /// <inheritdoc/>
  public async Task<Result> ValidateForCreate(Datasource datasource, Scenario domainEntity, CancellationToken ct)
  {
    var errors = new List<ValidationError>();

    var code = domainEntity.Code?.Trim() ?? string.Empty;
    if (!string.IsNullOrEmpty(code) &&
        await _listScenariosQueryService.ExistsByCodeAsync(datasource, code, ct))
    {
      errors.Add(new ValidationError(nameof(Scenario.Code),
        $"Kód scénáře '{code}' musí být unikátní v datasource {datasource}."));
    }

    if (await _listScenariosQueryService.ExistsByIdAsync(datasource, domainEntity.Id, ct))
    {
      errors.Add(new ValidationError(nameof(Scenario.Id),
        $"Scénář s Id '{domainEntity.Id}' již existuje v datasource {datasource}."));
    }

    return errors.Count == 0 ? Result.Success() : Result.Invalid(errors);
  }
}

