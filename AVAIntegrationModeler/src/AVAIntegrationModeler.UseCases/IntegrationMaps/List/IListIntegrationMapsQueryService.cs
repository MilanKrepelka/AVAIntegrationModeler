using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.IntegrationMaps.List;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IListIntegrationMapsQueryService
{
  Task<IEnumerable<IntegrationMapSummaryDTO>> ListSummaryAsync(Contracts.Datasource dataSource);

  Task<IEnumerable<IntegrationMapDTO>> ListAsync(Contracts.Datasource dataSource);
}
