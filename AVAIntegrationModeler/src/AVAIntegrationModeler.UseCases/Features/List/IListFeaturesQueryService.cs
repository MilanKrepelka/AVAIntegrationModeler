using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Features.List;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IListFeaturesQueryService
{
  Task<IEnumerable<FeatureDTO>> ListAsync(Contracts.Datasource dataSource);
}
