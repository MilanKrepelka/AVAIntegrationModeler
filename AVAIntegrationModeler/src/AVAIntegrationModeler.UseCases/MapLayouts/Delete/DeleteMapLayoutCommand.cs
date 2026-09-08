using Ardalis.Result;
using MediatR;

namespace AVAIntegrationModeler.UseCases.MapLayouts.Delete;

/// <summary>
/// Příkaz pro smazání MapLayout podle klíče.
/// </summary>
public record DeleteMapLayoutCommand(string Key) : IRequest<Result>;
