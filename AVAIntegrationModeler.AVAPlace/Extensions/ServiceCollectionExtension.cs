using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASOL.Core.ApiConnector;
using ASOL.DataIntegrationExternalAgent.API.Connectors;
using ASOL.DataService.Connector;
using ASOL.DataService.Connector.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AVAIntegrationModeler.AVAPlace.Extensions;
public static class ServiceCollectionExtension
{
  /// <summary>
  /// Represents the DataService client extension methods for service collection.
  /// </summary>
  /// <param name="services">The services collection.</param>
  /// <param name="options">The options for DataService client. (optional)</param>
  /// <returns>The services collection. (fluent API)</returns>
  private static IServiceCollection AddDataServiceClientExtended(this IServiceCollection services, Action<DataServiceClientOptions>? options = null)
  {
    //data-service client
    services.AddScoped<IDataServiceClient>(sp => sp.GetRequiredService<ICustomDataServiceClient>());

    services.AddScoped<ICustomDataServiceClient>(sp =>
    {
      var logger = sp.GetRequiredService<ILogger<CustomDataServiceClient>>();
      var tokenProvider = sp.GetRequiredService<IConnectorTokenProvider>();
      var requestHeaderProviders = sp.GetServices<IRequestHeaderProvider>();
      if (options == null)
      {
        var opt = sp.GetRequiredService<IOptions<DataServiceClientOptions>>();
        return new CustomDataServiceClient(opt, logger, tokenProvider, requestHeaderProviders);
      }
      else
      {
        var opt = DataServiceClientOptions.Default;
        options(opt);
        return new CustomDataServiceClient(Microsoft.Extensions.Options.Options.Create(opt), logger, tokenProvider, requestHeaderProviders);
      }
    });

    return services;
  }
}
