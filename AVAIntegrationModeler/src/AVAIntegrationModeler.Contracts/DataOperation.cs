using System.ComponentModel;

namespace AVAIntegrationModeler.Contracts;

/// <summary>
/// Vyjadřuje typ operace prováděné nad daty entity
/// </summary>
public enum DataOperation
{
  /// <summary>
  /// Vytvoření nové entity
  /// </summary>
  Create = 0,

  /// <summary>
  /// Aktualizace existující entity
  /// </summary>
  Update = 1,

  /// <summary>
  /// Smazání entity
  /// </summary>
  Delete = 2,

  /// <summary>
  /// Čtení entity
  /// </summary>
  Read = 3
}
