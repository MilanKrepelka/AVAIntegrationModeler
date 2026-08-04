using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Areas.List;

/// <summary>
/// Dotaz pro výpis oblastí.
/// </summary>
public record ListAreasQuery(Datasource Datasource) : IQuery<Result<IEnumerable<AreaDTO>>>;
