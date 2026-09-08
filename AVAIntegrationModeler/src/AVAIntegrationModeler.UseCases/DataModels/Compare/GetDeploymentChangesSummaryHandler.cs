using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DataModels;

namespace AVAIntegrationModeler.UseCases.DataModels.Compare;

/// <summary>
/// Handler pro souhrnné porovnání DataModelů nasazení mezi databází a AVAPlace.
/// DataModelIds jsou součástí dotazu — nevyžaduje DB lookup nasazení.
/// </summary>
public class GetDeploymentChangesSummaryHandler(IDataModelQueryService dataModelQueryService)
  : IQueryHandler<GetDeploymentChangesSummaryQuery, Result<DeploymentChangesSummaryDTO>>
{
  /// <inheritdoc />
  public async Task<Result<DeploymentChangesSummaryDTO>> Handle(
    GetDeploymentChangesSummaryQuery request, CancellationToken cancellationToken)
  {
    if (request.DataModelIds.Count == 0)
      return Result<DeploymentChangesSummaryDTO>.Success(new DeploymentChangesSummaryDTO
      {
        DeploymentCode = request.DeploymentCode,
        DeploymentName = request.DeploymentName,
        TotalModels = 0
      });

    var dbTask = dataModelQueryService.ListAsync(Datasource.Database);
    var avaTask = dataModelQueryService.ListAsync(Datasource.AVAPlace);
    await Task.WhenAll(dbTask, avaTask);

    var dbModels = dbTask.Result
      .GroupBy(m => m.Id)
      .ToDictionary(g => g.Key, g => g.First());
    var avaModels = avaTask.Result
      .Where(m => m.Id != Guid.Empty)
      .GroupBy(m => m.Id)
      .ToDictionary(g => g.Key, g => g.First());

    var items = new List<DataModelComparisonSummaryItemDTO>();
    foreach (var id in request.DataModelIds.Where(id => id != Guid.Empty))
    {
      dbModels.TryGetValue(id, out var dbModel);
      avaModels.TryGetValue(id, out var avaModel);

      var cmp = DataModelComparer.BuildComparison(id, dbModel, avaModel);
      items.Add(new DataModelComparisonSummaryItemDTO
      {
        DataModelId = id,
        Code = cmp.Code,
        Name = cmp.Name,
        Status = cmp.ModelStatus,
        HasDifferences = cmp.HasDifferences,
        PropertyDiffCount = cmp.PropertyDiffs.Count,
        FieldsDifferent = cmp.FieldComparisons.Count(f => f.Status == ComparisonStatus.Different),
        FieldsOnlyInDatabase = cmp.FieldComparisons.Count(f => f.Status == ComparisonStatus.OnlyInDatabase),
        FieldsOnlyInAvaPlace = cmp.FieldComparisons.Count(f => f.Status == ComparisonStatus.OnlyInAvaPlace)
      });
    }

    return Result<DeploymentChangesSummaryDTO>.Success(new DeploymentChangesSummaryDTO
    {
      DeploymentCode = request.DeploymentCode,
      DeploymentName = request.DeploymentName,
      TotalModels = items.Count,
      ModelsSame = items.Count(i => i.Status == ComparisonStatus.Same),
      ModelsDifferent = items.Count(i => i.Status == ComparisonStatus.Different),
      ModelsOnlyInDatabase = items.Count(i => i.Status == ComparisonStatus.OnlyInDatabase),
      ModelsOnlyInAvaPlace = items.Count(i => i.Status == ComparisonStatus.OnlyInAvaPlace),
      HasDifferences = items.Any(i => i.HasDifferences),
      Models = items.OrderBy(i => i.Code, StringComparer.OrdinalIgnoreCase).ToList()
    });
  }
}
