using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.UseCases.Scenarios.List;

namespace AVAIntegrationModeler.Infrastructure.ValidationServices;

/// <inheritdoc/>
public class ScenarioValidationService : IDomainEntityValidationService<Scenario>
{
  private readonly IListScenariosQueryService _listScenariosQueryService;

  public ScenarioValidationService(IListScenariosQueryService listScenariosQueryService)
  {
    _listScenariosQueryService = listScenariosQueryService;
  }

  /// <inheritdoc/>
  public Task<Result> Validate(Datasource datasource, Scenario domainEntity, CancellationToken ct)
    => Task.FromResult(Result.Success());

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

