using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DataModels;

namespace AVAIntegrationModeler.UseCases.DataModels.Compare;

/// <summary>
/// Handler pro porovnání datového modelu mezi databází a AVAPlace.
/// </summary>
public class CompareDataModelHandler(IDataModelQueryService queryService)
  : IQueryHandler<CompareDataModelQuery, Result<DataModelComparisonDTO>>
{
  /// <inheritdoc />
  public async Task<Result<DataModelComparisonDTO>> Handle(
    CompareDataModelQuery request, CancellationToken cancellationToken)
  {
    var dbTask = queryService.ListAsync(Datasource.Database);
    var avaTask = queryService.ListAsync(Datasource.AVAPlace);
    await Task.WhenAll(dbTask, avaTask);

    var dbModel = dbTask.Result.FirstOrDefault(m => m.Id == request.DataModelId);
    var avaModel = avaTask.Result.FirstOrDefault(m => m.Id == request.DataModelId);

    if (dbModel is null && avaModel is null)
      return Result<DataModelComparisonDTO>.NotFound($"DataModel s ID {request.DataModelId} nebyl nalezen v žádném zdroji.");

    return Result<DataModelComparisonDTO>.Success(
      DataModelComparer.BuildComparison(request.DataModelId, dbModel, avaModel));
  }
}
