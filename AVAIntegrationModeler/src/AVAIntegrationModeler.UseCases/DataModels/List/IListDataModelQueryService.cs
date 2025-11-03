using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModels.List;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IListDataModelQueryService
{
  Task<IEnumerable<DataModelSummaryDTO>> ListSummaryAsync(Contracts.Datasource dataSource);

  Task<IEnumerable<DataModelDTO>> ListAsync(Contracts.Datasource dataSource);
}
