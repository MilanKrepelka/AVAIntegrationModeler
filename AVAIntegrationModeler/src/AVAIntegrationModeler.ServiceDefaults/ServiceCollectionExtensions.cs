using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AVAIntegrationModeler.ServiceDefaults;
public static class ServiceCollectionExtensions
{
  /// <summary>
  /// Rozšiřující metoda pro registraci FluentClientFactory do DI kontejneru.
  /// </summary>
  /// <param name="services">Kolekce služeb pro DI kontejner.</param>
  /// <param name="configuration">Konfigurace aplikace, ze které se načítá základní URL API.</param>
  /// <returns>Vrací upravenou kolekci služeb.</returns>
  public static IServiceCollection AddFluentFactory(this IServiceCollection services, IConfiguration configuration)
  {
    services.TryAddSingleton<IFluentClientFactory>(sp => new FluentClientFactory(new FluentClientOptions()
    {
      BaseUrl = configuration["Api:BaseUrl"]
    }));
    return services;
  }
}
