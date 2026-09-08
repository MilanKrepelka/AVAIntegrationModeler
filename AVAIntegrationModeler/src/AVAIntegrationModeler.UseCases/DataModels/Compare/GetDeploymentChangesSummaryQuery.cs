using AVAIntegrationModeler.Contracts.DataModels;

namespace AVAIntegrationModeler.UseCases.DataModels.Compare;

/// <summary>
/// Dotaz pro souhrnné porovnání DataModelů nasazení mezi databází a AVAPlace.
/// DataModelIds jsou předány přímo — nevyžaduje DB lookup nasazení.
/// </summary>
public record GetDeploymentChangesSummaryQuery(
  string DeploymentCode,
  string DeploymentName,
  List<Guid> DataModelIds)
  : IQuery<Result<DeploymentChangesSummaryDTO>>;
