namespace AVAIntegrationModeler.UseCases.Areas.SaveMap;

/// <summary>
/// Příkaz pro uložení JSON diagramu mapy oblasti.
/// </summary>
/// <param name="AreaId">ID oblasti.</param>
/// <param name="DiagramJson">JSON stav diagramu ze SfDiagramComponent.SaveDiagram().</param>
public record SaveAreaMapCommand(Guid AreaId, string DiagramJson) : ICommand<Result>;
