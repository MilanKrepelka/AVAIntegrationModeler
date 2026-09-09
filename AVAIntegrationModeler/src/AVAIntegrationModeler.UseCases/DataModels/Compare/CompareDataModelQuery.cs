using AVAIntegrationModeler.Contracts.DataModels;

namespace AVAIntegrationModeler.UseCases.DataModels.Compare;

/// <summary>
/// Dotaz pro hluboké porovnání datového modelu mezi databází a AVAPlace.
/// </summary>
public record CompareDataModelQuery(Guid DataModelId) : IQuery<Result<DataModelComparisonDTO>>;
