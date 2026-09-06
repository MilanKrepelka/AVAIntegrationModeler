using Ardalis.Result;
using Ardalis.SharedKernel;
using AVAIntegrationModeler.Domain.MapLayoutAggregate;
using AVAIntegrationModeler.Domain.MapLayoutAggregate.Specs;
using MediatR;

namespace AVAIntegrationModeler.UseCases.MapLayouts.Save;

/// <summary>
/// Handler pro upsert MapLayout — vytvoří nový záznam nebo aktualizuje existující.
/// </summary>
public class SaveMapLayoutHandler(IRepository<MapLayout> repository)
  : IRequestHandler<SaveMapLayoutCommand, Result>
{
  public async Task<Result> Handle(SaveMapLayoutCommand request, CancellationToken cancellationToken)
  {
    var spec = new MapLayoutByKeySpec(request.Key);
    var layout = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (layout is null)
    {
      layout = new MapLayout(Guid.NewGuid(), request.Key);
      layout.SetDiagram(request.DiagramJson);
      await repository.AddAsync(layout, cancellationToken);
    }
    else
    {
      layout.SetDiagram(request.DiagramJson);
      await repository.UpdateAsync(layout, cancellationToken);
    }

    return Result.Success();
  }
}
