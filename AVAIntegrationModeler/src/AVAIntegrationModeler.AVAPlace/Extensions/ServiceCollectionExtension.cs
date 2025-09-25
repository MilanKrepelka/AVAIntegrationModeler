using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASOL.Core.ApiConnector;
using ASOL.Core.ApiConnector.Extensions;
using ASOL.Core.Identity.Options;
using ASOL.Core.Multitenancy.AspNetCore.Extensions;
using ASOL.DataService.Connector;
using ASOL.DataService.Connector.Options;
using ASOL.IdentityProvider.AspNetCore.Extensions;
using ASOL.IdentityProvider.Connector.Options;
using AVAIntegrationModeler.AVAPlace.API.Connectors;
using AVAIntegrationModeler.AVAPlace.Options;
using Microsoft.Extensions.Configuration;
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
  public static IServiceCollection AddDataServiceClientExtended(this IServiceCollection services, Action<DataServiceClientOptions>? options = null)
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
  public static IServiceCollection AddMultitenancySetup(this IServiceCollection services)
  {
    services
        .AddMultitenancy()
        .AddAuthorization();

    return services;
  }
  public static IServiceCollection AddApiConnectors(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<SsoAuthOptions>(configuration.GetSection(SsoAuthOptions.DefaultSectionName));
    
    services.AddStandardApiTokenProviders();

    services.Configure<IdentityProviderClientOptions>(configuration.GetSection(nameof(IdentityProviderClientOptions)));
    services.AddIdentityProviderClient();

    // add dataservice clients and infrastructure for data integrations
    services.Configure<DataServiceQueryOptions>(configuration.GetSection(nameof(DataServiceQueryOptions)));
    services.Configure<DataServiceClientOptions>(configuration.GetSection(nameof(DataServiceClientOptions)));
    services.AddDataServiceClientExtended();
    /*
    services.AddTransient<IDataServiceIntegrationConnector, DataServiceIntegrationConnector>();
    services.AddTransient<IDataServiceConfigurationTenantCache, DataServiceConfigurationTenantCache>();
    services.AddTransient<NonTenantIdentityProviderClient>();
    services.AddTransient<IDataSynchronizationService, ExampleDataSynchronizationService>(); //TODO
    */
    return services;
  }
}
