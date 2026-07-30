using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Areas.Update;

/// <summary>
/// Příkaz pro aktualizaci oblasti.
/// </summary>
/// <param name="Datasource">Datový zdroj, ve kterém se oblast nachází.</param>
/// <param name="Area">Aktualizovaná oblast.</param>
public record UpdateAreaCommand(Datasource Datasource, AreaDTO Area) : ICommand<Result<AreaDTO>>;
