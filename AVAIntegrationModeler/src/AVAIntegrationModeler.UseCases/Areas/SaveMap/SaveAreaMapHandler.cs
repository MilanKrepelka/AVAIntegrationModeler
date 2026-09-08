using AVAIntegrationModeler.Domain.AreaAggregate;

namespace AVAIntegrationModeler.UseCases.Areas.SaveMap;

/// <summary>
/// Handler pro uložení JSON diagramu mapy oblasti.
/// </summary>
public class SaveAreaMapHandler(IRepository<Area> repository) : ICommandHandler<SaveAreaMapCommand, Result>
{
  public async Task<Result> Handle(SaveAreaMapCommand request, CancellationToken cancellationToken)
  {
    var area = await repository.GetByIdAsync(request.AreaId, cancellationToken);
    if (area is null)
      return Result.NotFound();

    area.SetMapDiagram(request.DiagramJson);
    await repository.UpdateAsync(area, cancellationToken);

    return Result.Success();
  }
}
