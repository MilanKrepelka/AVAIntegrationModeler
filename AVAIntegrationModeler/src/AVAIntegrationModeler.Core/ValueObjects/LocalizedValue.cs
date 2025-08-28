using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Core.ValueObjects;

/// <summary>
/// Reprezentuje hodnotu, kterou lze lokalizovat do více jazyků.
/// </summary>
/// <remarks>Tato třída poskytuje vlastnosti pro ukládání lokalizovaných hodnot v konkrétních jazycích, jako je čeština a angličtina. Je určena pro scénáře, kde je vyžadována vícejazyčná podpora.</remarks>
public class LocalizedValue : IEquatable<LocalizedValue>
{
    /// <summary>
    /// Lokalizovaná hodnota v češtině.
    /// </summary>
    public string CzechValue { get; set; } = string.Empty;

    /// <summary>
    /// Lokalizovaná hodnota v angličtině.
    /// </summary>
    public string EnglishValue { get; set; } = string.Empty;

    public bool Equals(LocalizedValue? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return CzechValue == other.CzechValue && EnglishValue == other.EnglishValue;
    }
}
