using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.DataModels.Export;

public record ExportDataModelsQuery(Datasource Datasource, List<Guid> ModelIds)
  : IQuery<Result<ExportResult<object>>>;
