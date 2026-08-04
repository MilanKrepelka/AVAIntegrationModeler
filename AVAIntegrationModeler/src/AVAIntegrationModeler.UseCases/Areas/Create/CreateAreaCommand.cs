using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Areas.Create;

/// <summary>
/// Příkaz pro vytvoření nové oblasti.
/// </summary>
/// <param name="Datasource"><see cref="AVAIntegrationModeler.Contracts.Datasource"/></param>
/// <param name="Area"><see cref="AreaDTO"/></param>
public record CreateAreaCommand(
    Datasource Datasource,
    AreaDTO Area
) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
