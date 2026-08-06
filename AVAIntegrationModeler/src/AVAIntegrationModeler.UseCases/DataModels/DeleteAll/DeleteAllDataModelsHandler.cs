using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModelRecords;

namespace AVAIntegrationModeler.UseCases.DataModels.DeleteAll;

/// <summary>
/// Handler pro smazání všech datových modelů z lokální databáze včetně jejich záznamů.
/// Nejprve smaže všechny DataModelRecordy (a jejich pole), poté všechny DataModely (a jejich pole).
/// </summary>
public class DeleteAllDataModelsHandler(
  IDataModelRepository dataModelRepository,
  IDataModelRecordRepository recordRepository,
  IDataModelQueryService dataModelQueryService,
  IDataModelRecordQueryService dataModelRecordQueryService)
  : ICommandHandler<DeleteAllDataModelsCommand, Result<DeleteAllDataModelsResponse>>
{
  public async Task<Result<DeleteAllDataModelsResponse>> Handle(
    DeleteAllDataModelsCommand request, CancellationToken cancellationToken)
  {
    var deletedRecordsCount = await recordRepository.DeleteAllAsync(cancellationToken);
    var deletedModelsCount = await dataModelRepository.DeleteAllAsync(cancellationToken);

    dataModelRecordQueryService.InvalidateCache(Datasource.Database);
    dataModelQueryService.InvalidateCache(Datasource.Database);

    return Result<DeleteAllDataModelsResponse>.Success(new DeleteAllDataModelsResponse
    {
      DeletedModelsCount = deletedModelsCount,
      DeletedRecordsCount = deletedRecordsCount
    });
  }
}
