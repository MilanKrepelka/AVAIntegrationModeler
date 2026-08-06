using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModels.Import;

/// <summary>
/// Handler pro hromadný import všech datových modelů z AVAPlace do lokální databáze,
/// včetně jejich DataModelRecordů. Import jednoho modelu, který selže, nezastaví
/// import ostatních modelů (continue-on-error) — chyba se promítne do výsledku daného modelu.
/// </summary>
public class ImportAllDataModelsHandler(
  IIntegrationDataProvider integrationDataProvider,
  IDataModelImportService importService,
  IDataModelQueryService queryService)
  : ICommandHandler<ImportAllDataModelsCommand, Result<ImportAllDataModelsFromAvaPlaceResponse>>
{
  public async Task<Result<ImportAllDataModelsFromAvaPlaceResponse>> Handle(
    ImportAllDataModelsCommand request, CancellationToken cancellationToken)
  {
    IEnumerable<DataModelDTO> models;
    try
    {
      models = await integrationDataProvider.GetDataModelsAsync(cancellationToken);
    }
    catch (Exception ex)
    {
      return Result<ImportAllDataModelsFromAvaPlaceResponse>.Error(
        $"Nepodařilo se načíst datové modely z AVAPlace: {ex.Message}");
    }

    var results = new List<ImportDataModelResultDTO>();
    foreach (var dto in models)
    {
      try
      {
        var importResult = await importService.ImportModelAsync(dto, cancellationToken);
        results.Add(new ImportDataModelResultDTO
        {
          AvaPlaceModelId = dto.Id,
          Code = dto.Code,
          Success = importResult.IsSuccess,
          LocalDataModelId = importResult.IsSuccess ? importResult.Value : null,
          ErrorMessage = importResult.IsSuccess ? null : string.Join("; ", importResult.Errors)
        });
      }
      catch (Exception ex)
      {
        results.Add(new ImportDataModelResultDTO
        {
          AvaPlaceModelId = dto.Id,
          Code = dto.Code,
          Success = false,
          ErrorMessage = ex.Message
        });
      }
    }

    if (results.Any(r => r.Success))
      queryService.InvalidateCache(Datasource.Database);

    return Result<ImportAllDataModelsFromAvaPlaceResponse>.Success(new ImportAllDataModelsFromAvaPlaceResponse
    {
      Results = results,
      SuccessCount = results.Count(r => r.Success),
      FailedCount = results.Count(r => !r.Success)
    });
  }
}
