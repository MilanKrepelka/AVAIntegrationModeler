using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Domain.AreaAggregate.Specifications;

namespace AVAIntegrationModeler.UseCases.Areas.Delete;

/// <summary>
/// Handler pro příkaz smazání oblasti.
/// </summary>
public class DeleteAreaHandler(IRepository<Area> repository, IAreasQueryService areasQueryService)
  : ICommandHandler<DeleteAreaCommand, Result>
{
  public async Task<Result> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
  {
    var area = await repository.FirstOrDefaultAsync(new AreaByCodeSpec(request.AreaCode), cancellationToken);
    if (area == null)
      return Result.NotFound();

    await repository.DeleteAsync(area, cancellationToken);
    areasQueryService.InvalidateCache(Datasource.Database);
    return Result.Success();
  }
}
