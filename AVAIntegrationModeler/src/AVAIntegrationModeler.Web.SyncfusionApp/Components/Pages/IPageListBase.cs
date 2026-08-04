using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

/// <summary>
/// Rozhraní pro základní vlastnosti stránkových komponent s výpisem položek.
/// </summary>
public interface IPageListBase
{
  /// <summary>
  /// Příznak indikující probíhající načítání dat.
  /// </summary>
  bool IsLoading { get; set; }

  /// <summary>
  /// Zdroj dat pro načítání položek.
  /// </summary>
  Datasource Datasource { get; set; }

  /// <summary>
  /// Filtrační řetězec pro vyhledávání položek.
  /// </summary>
  string FilterString { get; set; } 


}
