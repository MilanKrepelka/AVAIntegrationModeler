using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using AVAIntegrationModeler.Contracts.Options;

namespace AVAIntegrationModeler.API.Client;

public static class ApiClientServiceCollectionExtensions
{
  public static IServiceCollection AddAVAIntegrationModelerApiClient(this IServiceCollection services, IConfiguration configuration)
  {
    
    // Bind options (section name matches your appsettings file in API.Client)
    services.Configure<AVAIntegrationModelerAPIOptions>(configuration.GetSection(nameof(AVAIntegrationModelerAPIOptions)));

    // Resolve a snapshot of options for immediate registration values
    var opts = configuration.GetSection(nameof(AVAIntegrationModelerAPIOptions)).Get<AVAIntegrationModelerAPIOptions>() ?? new AVAIntegrationModelerAPIOptions();

    // Register typed HttpClient for the API client
    services.AddHttpClient<IAVAIntegrationModelerApiClient, AVAIntegrationModelerApiClient>(client =>
    {
      if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
      {
        client.BaseAddress = new Uri(opts.BaseUrl);
      }
      client.Timeout = TimeSpan.FromSeconds(opts.TimeoutInSeconds);
    });

    // Samostatný HttpClient s delším timeoutem pro hromadný import datových modelů z AVAPlace —
    // ten může u velkého počtu modelů trvat výrazně déle než běžná volání API.
    services.AddHttpClient(AVAIntegrationModelerApiClient.BulkImportHttpClientName, client =>
    {
      if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
      {
        client.BaseAddress = new Uri(opts.BaseUrl);
      }
      client.Timeout = TimeSpan.FromSeconds(opts.BulkImportTimeoutInSeconds);
    });

    return services;
  }
}
