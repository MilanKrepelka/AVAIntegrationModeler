using Ardalis.Result;
using Ardalis.SharedKernel;
using AVAIntegrationModeler.Domain.MapLayoutAggregate;
using AVAIntegrationModeler.Domain.MapLayoutAggregate.Specs;
using MediatR;

namespace AVAIntegrationModeler.UseCases.MapLayouts.Delete;

/// <summary>
/// Handler pro smazání MapLayout podle klíče.
/// </summary>
public class DeleteMapLayoutHandler(IRepository<MapLayout> repository)
  : IRequestHandler<DeleteMapLayoutCommand, Result>
{
  public async Task<Result> Handle(DeleteMapLayoutCommand request, CancellationToken cancellationToken)
  {
    var spec = new MapLayoutByKeySpec(request.Key);
    var layout = await repository.FirstOrDefaultAsync(spec, cancellationToken);

    if (layout is null)
      return Result.NotFound();

    await repository.DeleteAsync(layout, cancellationToken);
    return Result.Success();
  }
}
