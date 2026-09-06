using Ardalis.Result;
using MediatR;

namespace AVAIntegrationModeler.UseCases.MapLayouts.Save;

/// <summary>
/// Příkaz pro uložení JSON diagramu MapLayout (upsert podle klíče).
/// </summary>
public record SaveMapLayoutCommand(string Key, string DiagramJson) : IRequest<Result>;
