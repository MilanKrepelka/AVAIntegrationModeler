using System.Net.Http;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Testovací implementace <see cref="IHttpClientFactory"/>, která pro jakýkoli název
/// vrací jeden a tentýž předem vytvořený <see cref="HttpClient"/> (typicky z
/// <see cref="Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory{TEntryPoint}"/>).
/// </summary>
public class TestHttpClientFactory(HttpClient httpClient) : IHttpClientFactory
{
  public HttpClient CreateClient(string name) => httpClient;
}
