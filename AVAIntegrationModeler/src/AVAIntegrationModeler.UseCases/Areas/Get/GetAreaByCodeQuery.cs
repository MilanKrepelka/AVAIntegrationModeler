using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Areas.Get;

/// <summary>
/// Dotaz pro načtení oblasti podle kódu.
/// </summary>
public record GetAreaByCodeQuery(Datasource Datasource, string AreaCode) : IQuery<Result<AreaDTO>>;
