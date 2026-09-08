using Ardalis.Result;
using Ardalis.SharedKernel;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.MapLayoutAggregate;
using AVAIntegrationModeler.Domain.MapLayoutAggregate.Specs;
using MediatR;

namespace AVAIntegrationModeler.UseCases.MapLayouts.Get;

/// <summary>
/// Handler pro načtení MapLayout podle klíče.
/// </summary>
public class GetMapLayoutHandler(IReadRepository<MapLayout> repository)
  : IRequestHandler<GetMapLayoutQuery, Result<MapLayoutDTO>>
{
  public async Task<Result<MapLayoutDTO>> Handle(GetMapLayoutQuery request, CancellationToken cancellationToken)
  {
    var spec = new MapLayoutByKeySpec(request.Key);
    var layout = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (layout is null)
      return Result<MapLayoutDTO>.NotFound();

    return Result<MapLayoutDTO>.Success(new MapLayoutDTO
    {
      Key = layout.Key,
      DiagramJson = layout.DiagramJson,
      LastMapSave = layout.LastMapSave
    });
  }
}
