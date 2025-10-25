using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Domain.ValueObjects;

/// <summary>
/// Reprezentuje hodnotu, kterou lze lokalizovat do více jazyků.
/// </summary>
/// <remarks>Tato třída poskytuje vlastnosti pro ukládání lokalizovaných hodnot v konkrétních jazycích, jako je čeština a angličtina. Je určena pro scénáře, kde je vyžadována vícejazyčná podpora.</remarks>
public class LocalizedValue : AVAIntegrationModeler.Contracts.DTO.LocalizedValue, IEquatable<LocalizedValue>
{
  public bool Equals(LocalizedValue? other)
  {
    if (other is null)
      return false;
    if (ReferenceEquals(this, other))
      return true;
    return CzechValue == other.CzechValue && EnglishValue == other.EnglishValue;
  }
}
