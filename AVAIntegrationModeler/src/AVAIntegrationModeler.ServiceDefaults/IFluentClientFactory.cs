using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.ServiceDefaults;

/// <summary>
/// Rozhraní pro továrnu FluentClientu.
/// Implementace této továrny by měla poskytovat instanci <see cref="Pathoschild.Http.Client.IClient"/>, 
/// která umožňuje provádět HTTP požadavky pomocí Fluent API.
/// </summary>
public interface IFluentClientFactory
{
    /// <summary>
    /// Vytvoří novou instanci <see cref="Pathoschild.Http.Client.IClient"/>.
    /// </summary>
    /// <returns>Nová instance <see cref="Pathoschild.Http.Client.IClient"/>.</returns>
    Pathoschild.Http.Client.IClient CreateClient();
}
