using System.ComponentModel;

namespace AVAIntegrationModeler.Contracts;

/// <summary>
/// Vyjadřuje typ zdroje dat. V principu může být databáze, AVAPlace, nebo jiný typ zdroje.
/// </summary>
public enum Datasource
{
  /// <summary>
  /// Data pochází z lokální databáze
  /// </summary>
  Database = 0,
  /// <summary>
  /// Data pochází z AVA Place
  /// </summary>
  AVAPlace = 1,
}
