using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Contributors;
using AVAIntegrationModeler.UseCases.Contributors.List;
using AVAIntegrationModeler.UseCases.Scenarios;
using AVAIntegrationModeler.UseCases.Scenarios.List;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

public class ListScenariosQueryService(AppDbContext _db, IIntegrationDataProvider integrationDataProvider) : IListScenariosQueryService
{
  public async Task<IEnumerable<ScenarioDTO>> ListAsync(Datasource datasouce)
  {
    List<ScenarioDTO> result = new List<ScenarioDTO>();
    if (datasouce == Datasource.AVAPlace)
    {
      result = (await integrationDataProvider.GetIntegrationScenariosAsync(CancellationToken.None)).ToList();
    }
    else
    {
      result = await _db.Scenarios
            .Select(s => UseCases.Scenarios.Mapping.ScenarioMapper.MapToDTO(s))
            .ToListAsync();

    }
    return result;
  }
    
}
