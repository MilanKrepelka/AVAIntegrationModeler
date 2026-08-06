using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Domain.AreaAggregate.Specifications;
using AVAIntegrationModeler.UseCases.DataModels;

namespace AVAIntegrationModeler.UseCases.Areas.Delete;

/// <summary>
/// Handler pro příkaz smazání oblasti. DataModely a IntegrationsMapy, které na oblast
/// odkazovaly, mají AreaId nastaveno na null pomocí FK OnDelete(SetNull) v databázi.
/// </summary>
public class DeleteAreaHandler(
  IRepository<Area> repository,
  IAreasQueryService areasQueryService,
  IDataModelQueryService dataModelQueryService)
  : ICommandHandler<DeleteAreaCommand, Result>
{
  public async Task<Result> Handle(DeleteAreaCommand request, CancellationToken cancellationToken)
  {
    var area = await repository.FirstOrDefaultAsync(new AreaByCodeSpec(request.AreaCode), cancellationToken);
    if (area == null)
      return Result.NotFound();

    await repository.DeleteAsync(area, cancellationToken);
    areasQueryService.InvalidateCache(Datasource.Database);
    // DataModely, jejichž AreaId bylo databází vynulováno, mají zastaralý (cachovaný) AreaId.
    dataModelQueryService.InvalidateCache(Datasource.Database);
    return Result.Success();
  }
}
