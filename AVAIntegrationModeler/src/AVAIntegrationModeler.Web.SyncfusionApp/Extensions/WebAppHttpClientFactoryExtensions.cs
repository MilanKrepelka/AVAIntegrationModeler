using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AVAIntegrationModeler.Web.SyncfusionApp;

public class WebAppHttpClientFactory
{
  private readonly IHttpClientFactory _factory;
  private readonly ILogger<WebAppHttpClientFactory> _logger;
  private readonly Uri? _defaultBaseAddress;

  public WebAppHttpClientFactory(IHttpClientFactory factory, IConfiguration config, ILogger<WebAppHttpClientFactory> logger)
  {
    _factory = factory;
    _logger = logger;

    // read base URL from config keys commonly used in this repo
    var baseUrl = config["WebApp:ApiBaseUrl"]
                  ?? config["Urls"]
                  ?? config["ASPNETCORE_URLS"]
                  ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

    if (!string.IsNullOrWhiteSpace(baseUrl))
    {
      // keep only first address if multiple are provided (comma or ';' separated)
      var candidate = baseUrl.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();

      if (!candidate.Contains("://", StringComparison.Ordinal))
      {
        candidate = candidate.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? candidate : "http://" + candidate;
      }

      if (Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
      {
        _defaultBaseAddress = uri;
      }
      else
      {
        _logger.LogWarning("Invalid base URL configured for WebAppHttpClientFactory: {BaseUrl}", candidate);
      }
    }
  }

  // Creates a client using the registered named client (or default) and applies configured BaseAddress when available.
  public HttpClient CreateClient(string name = "default")
  {
    var client = _factory.CreateClient(name);
    if (_defaultBaseAddress is not null)
    {
      client.BaseAddress = _defaultBaseAddress;
    }
    return client;
  }
}
