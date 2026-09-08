using Ardalis.Result;
using AVAIntegrationModeler.Contracts.DTO;
using MediatR;

namespace AVAIntegrationModeler.UseCases.MapLayouts.Get;

/// <summary>
/// Dotaz pro načtení MapLayout podle klíče.
/// </summary>
public record GetMapLayoutQuery(string Key) : IRequest<Result<MapLayoutDTO>>;
