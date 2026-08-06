using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate.Specifications;

namespace AVAIntegrationModeler.Infrastructure.ValidationServices;

/// <inheritdoc/>
public class DataModelValidationService(IRepository<DataModel> _repository) : IDomainEntityValidationService<DataModel>
{
  /// <inheritdoc/>
  public async Task<Result> Validate(Datasource datasource, DataModel domainEntity, CancellationToken ct)
  {
    var existingByCode = await _repository.FirstOrDefaultAsync(new DataModelByCodeSpec(domainEntity.Code), ct);
    if (existingByCode is not null && existingByCode.Id != domainEntity.Id)
      return Result.Conflict($"Datový model s kódem '{domainEntity.Code}' již existuje.");

    return Result.Success();
  }

  /// <inheritdoc/>
  public async Task<Result> ValidateForCreate(Datasource datasource, DataModel domainEntity, CancellationToken ct)
  {
    var existingByCode = await _repository.FirstOrDefaultAsync(new DataModelByCodeSpec(domainEntity.Code), ct);
    if (existingByCode is not null)
      return Result.Conflict($"Datový model s kódem '{domainEntity.Code}' již existuje.");

    var existingById = await _repository.GetByIdAsync(domainEntity.Id, ct);
    if (existingById is not null)
      return Result.Invalid(new ValidationError(nameof(DataModel.Id), $"Datový model s Id '{domainEntity.Id}' již existuje."));

    return Result.Success();
  }
}
