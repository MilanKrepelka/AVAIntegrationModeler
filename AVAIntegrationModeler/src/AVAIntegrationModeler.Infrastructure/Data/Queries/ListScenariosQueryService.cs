using AVAIntegrationModeler.UseCases.Contributors;
using AVAIntegrationModeler.UseCases.Contributors.List;
using AVAIntegrationModeler.UseCases.Scenarios;
using AVAIntegrationModeler.UseCases.Scenarios.List;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

public class ListScenariosQueryService(AppDbContext _db) : IListScenariosQueryService
{
    public async Task<IEnumerable<ScenarioDTO>> ListAsync()
    {
        List<ScenarioDTO> result = await _db.Scenarios
          .Select(s => UseCases.Scenarios.Mapping.ScenarioMapper.MapToDTO(s))
          .ToListAsync();
        return result;
    }
}
