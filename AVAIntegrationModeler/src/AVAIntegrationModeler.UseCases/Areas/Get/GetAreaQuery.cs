using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Areas.Get;

/// <summary>
/// Dotaz pro načtení oblasti podle identifikátoru.
/// </summary>
public record GetAreaQuery(Datasource Datasource, Guid AreaId) : IQuery<Result<AreaDTO>>;
