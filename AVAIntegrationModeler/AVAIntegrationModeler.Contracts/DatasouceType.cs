using System.ComponentModel;

namespace AVAIntegrationModeler.Contracts;

/// <summary>
/// Vyjadřuje typ zdroje dat. V principu může být databáze, AVAPlace, nebo jiný typ zdroje.
/// </summary>
public enum Datasource
{
  Database = 0,
  AVAPlace = 1,
}
