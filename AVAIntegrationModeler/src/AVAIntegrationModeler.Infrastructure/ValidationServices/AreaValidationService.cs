using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Domain.AreaAggregate.Specifications;

namespace AVAIntegrationModeler.Infrastructure.ValidationServices;

/// <inheritdoc/>
public class AreaValidationService(IRepository<Area> _repository) : IDomainEntityValidationService<Area>
{
  /// <inheritdoc/>
  public async Task<Result> Validate(Datasource datasource, Area domainEntity, CancellationToken ct)
  {
    var existingByCode = await _repository.FirstOrDefaultAsync(new AreaByCodeSpec(domainEntity.Code), ct);
    if (existingByCode is not null && existingByCode.Id != domainEntity.Id)
      return Result.Conflict($"Oblast s kódem '{domainEntity.Code}' již existuje.");

    return Result.Success();
  }

  /// <inheritdoc/>
  public async Task<Result> ValidateForCreate(Datasource datasource, Area domainEntity, CancellationToken ct)
  {
    var existingByCode = await _repository.FirstOrDefaultAsync(new AreaByCodeSpec(domainEntity.Code), ct);
    if (existingByCode is not null)
      return Result.Conflict($"Oblast s kódem '{domainEntity.Code}' již existuje.");

    var existingById = await _repository.GetByIdAsync(domainEntity.Id, ct);
    if (existingById is not null)
      return Result.Invalid(new ValidationError(nameof(Area.Id), $"Oblast s Id '{domainEntity.Id}' již existuje."));

    return Result.Success();
  }
}
