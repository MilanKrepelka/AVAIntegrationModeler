namespace AVAIntegrationModeler.Contracts;

/// <summary>
/// Požadavek na hromadný import všech datových modelů z AVAPlace do lokální databáze.
/// </summary>
public class ImportAllDataModelsFromAvaPlaceRequest
{
  /// <summary>
  /// Routa endpointu pro hromadný import datových modelů z AVAPlace.
  /// </summary>
  public const string Route = "/DataModels/ImportAllFromAVAPlace";
}
