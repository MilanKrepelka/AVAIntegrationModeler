using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DeploymentAggregate.Specifications;
using AVAIntegrationModeler.UseCases.Deployments.Mapping;

namespace AVAIntegrationModeler.UseCases.Deployments.CompleteDevelopment;

/// <summary>
/// Handler pro příkaz ukončení vývoje nasazení.
/// </summary>
public class CompleteDeploymentDevelopmentHandler(
  IDeploymentRepository repository,
  IDeploymentsQueryService queryService
) : ICommandHandler<CompleteDeploymentDevelopmentCommand, Result<DeploymentDTO>>
{
  /// <summary>
  /// Načte nasazení podle kódu, nastaví datum posledního nasazení na aktuální čas (UTC) a uloží jej.
  /// </summary>
  public async Task<Result<DeploymentDTO>> Handle(CompleteDeploymentDevelopmentCommand request, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(request.Code))
      return Result<DeploymentDTO>.Invalid(new ValidationError
      {
        Identifier = nameof(request.Code),
        ErrorMessage = "Kód nasazení je povinný."
      });

    var existing = await repository.FirstOrDefaultAsync(
      new DeploymentByCodeSpec(request.Code), cancellationToken);
    if (existing is null)
      return Result<DeploymentDTO>.NotFound();

    existing.CompleteDevelopment(DateTime.UtcNow);

    // DTO sestavíme před uložením — reset stavu entity v SyncDataModelsAndSaveAsync
    // může vrátit skalární hodnoty na původní (viz UpdateDeploymentHandler).
    var resultDto = DeploymentMapper.MapToDTO(existing)!;

    await repository.SyncDataModelsAndSaveAsync(existing, [], [], cancellationToken);
    queryService.InvalidateCache(Datasource.Database);

    return Result<DeploymentDTO>.Success(resultDto);
  }
}
