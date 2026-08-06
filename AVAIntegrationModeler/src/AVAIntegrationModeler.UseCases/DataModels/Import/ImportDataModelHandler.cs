using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.UseCases.DataModels.Import;

/// <summary>
/// Handler pro import jednoho datového modelu z AVAPlace do lokální databáze.
/// Načte model z AVAPlace a delegoval samotný upsert na <see cref="IDataModelImportService"/>.
/// </summary>
public class ImportDataModelHandler(
  IIntegrationDataProvider integrationDataProvider,
  IDataModelImportService importService,
  IDataModelQueryService queryService)
  : ICommandHandler<ImportDataModelCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(ImportDataModelCommand request, CancellationToken cancellationToken)
  {
    var dto = await integrationDataProvider.GetDataModelByIdAsync(request.AvaPlaceModelId, cancellationToken);
    if (dto is null)
      return Result<Guid>.NotFound();

    var result = await importService.ImportModelAsync(dto, cancellationToken);
    if (result.IsSuccess)
      queryService.InvalidateCache(Datasource.Database);

    return result;
  }
}
