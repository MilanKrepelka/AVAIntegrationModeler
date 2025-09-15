using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathoschild.Http.Client;

namespace AVAIntegrationModeler.ServiceDefaults;

/// <summary>
/// Tovární třída pro vytváření instancí <see cref="FluentClient"/>.
/// Umožňuje konfigurovat základní URL klienta pomocí <see cref="FluentClientOptions"/>.
/// </summary>
/// <remarks>
/// Inicializuje novou instanci třídy <see cref="FluentClientFactory"/> s danými možnostmi klienta.
/// </remarks>
/// <param name="fluentClientOptions">Možnosti konfigurace klienta, včetně základní URL.</param>
public class FluentClientFactory(FluentClientOptions fluentClientOptions) : IFluentClientFactory
{
    private readonly FluentClientOptions _fluentClientOptions = fluentClientOptions;

  /// <summary>
  /// Tovární metoda pro vytvoření nové instance <see cref="FluentClient"/>.
  /// </summary>
  /// <returns>Nová instance <see cref="IClient"/> s nastavenou základní URL.</returns>
  public Pathoschild.Http.Client.IClient CreateClient()
    {
        return new FluentClient(_fluentClientOptions.BaseUrl);
    }
}
