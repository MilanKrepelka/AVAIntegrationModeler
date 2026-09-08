namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Rozdíl hodnoty jedné vlastnosti mezi databází a AVAPlace.
/// </summary>
public record DataModelPropertyDiffDTO(
  string PropertyName,
  string? DatabaseValue,
  string? AvaPlaceValue);
