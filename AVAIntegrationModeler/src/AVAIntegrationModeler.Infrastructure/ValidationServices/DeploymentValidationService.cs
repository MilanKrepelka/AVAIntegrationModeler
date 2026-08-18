using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.Domain.DeploymentAggregate.Specifications;

namespace AVAIntegrationModeler.Infrastructure.ValidationServices;

/// <inheritdoc/>
public class DeploymentValidationService(IRepository<Deployment> _repository) : IDomainEntityValidationService<Deployment>
{
  /// <inheritdoc/>
  public async Task<Result> Validate(Datasource datasource, Deployment domainEntity, CancellationToken ct)
  {
    var existingByCode = await _repository.FirstOrDefaultAsync(new DeploymentByCodeSpec(domainEntity.Code), ct);
    if (existingByCode is not null && existingByCode.Id != domainEntity.Id)
      return Result.Conflict($"Nasazení s kódem '{domainEntity.Code}' již existuje.");

    return Result.Success();
  }

  /// <inheritdoc/>
  public async Task<Result> ValidateForCreate(Datasource datasource, Deployment domainEntity, CancellationToken ct)
  {
    var existingByCode = await _repository.FirstOrDefaultAsync(new DeploymentByCodeSpec(domainEntity.Code), ct);
    if (existingByCode is not null)
      return Result.Conflict($"Nasazení s kódem '{domainEntity.Code}' již existuje.");

    var existingById = await _repository.GetByIdAsync(domainEntity.Id, ct);
    if (existingById is not null)
      return Result.Invalid(new ValidationError(nameof(Deployment.Id), $"Nasazení s Id '{domainEntity.Id}' již existuje."));

    return Result.Success();
  }
}
