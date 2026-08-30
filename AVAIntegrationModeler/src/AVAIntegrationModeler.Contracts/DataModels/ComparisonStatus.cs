namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Stav porovnání záznamu mezi dvěma datovými zdroji.
/// </summary>
public enum ComparisonStatus
{
  /// <summary>Záznam je v obou zdrojích shodný.</summary>
  Same = 0,
  /// <summary>Záznam existuje v obou zdrojích, ale liší se.</summary>
  Different = 1,
  /// <summary>Záznam existuje pouze v lokální databázi.</summary>
  OnlyInDatabase = 2,
  /// <summary>Záznam existuje pouze v AVAPlace.</summary>
  OnlyInAvaPlace = 3
}
