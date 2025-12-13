using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

// Ensure the test project references the API project so `Program` is available.
// using AVAIntegrationModeler.API; // If Program is in a different namespace adjust this using accordingly.

namespace AVAIntegrationModeler.API.Client.Test;

public class AVAIntegrationModelerApiClientTest : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private readonly AVAIntegrationModelerAPIFactory _factory;

  public AVAIntegrationModelerApiClientTest(AVAIntegrationModelerAPIFactory factory)
  {
    _factory = factory;
  }

  [Fact]
  public async Task GetDataModels_Returns_Any_DataModels()
  {
    // Arrange: create HttpClient for the in-memory server
    using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions() { BaseAddress = new Uri("http://0.0.0.0:5005") });

    // Create the typed client wrapper
    IAVAIntegrationModelerApiClient apiClient = new AVAIntegrationModelerApiClient(client, NullLogger<AVAIntegrationModelerApiClient>.Instance);

    // Act: call the API method
    var result = await apiClient.GetDataModels(Contracts.Datasource.Database, CancellationToken.None);

    // Assert: ensure we got a non-null, non-empty response from the API
    Assert.NotNull(result);
    Assert.NotNull(result.DataModels);
    Assert.NotEmpty(result.DataModels);
  }
}
