using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.UseCases;
/// <summary>
/// Příznakové rozhraní pro dotazovací služby, které podporují cachování.
/// </summary>
public interface ICacheableQueryService
{
  /// <summary>
  /// Zruší aktuální cache a zneplatní všechna uložená data.
  /// </summary>
  /// <remarks>Tato metoda invaliduje cache a znemožní přístup k dříve uloženým datům. Před voláním této metody
  /// zajistěte dokončení všech závislých operací.</remarks>
  public void InvalidateCache(Datasource datasource);
}
