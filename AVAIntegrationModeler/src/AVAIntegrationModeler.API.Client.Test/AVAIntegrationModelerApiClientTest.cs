using System;
using System.Net.Http;
using System.Threading.Tasks;
using Ardalis.Result;
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
    IAVAIntegrationModelerApiClient apiClient = new AVAIntegrationModelerApiClient(client, new TestHttpClientFactory(client), NullLogger<AVAIntegrationModelerApiClient>.Instance);

    // Act: call the API method
    var result = await apiClient.GetDataModels(Contracts.Datasource.Database, CancellationToken.None);

    // Assert: ensure we got a non-null, non-empty response from the API
    Assert.NotNull(result);
    Assert.NotNull(result.DataModels);
    Assert.NotEmpty(result.DataModels);

    // Act: call the API method
    result = await apiClient.GetDataModels(Contracts.Datasource.AVAPlace, CancellationToken.None);

    // Assert: ensure we got a non-null, non-empty response from the API
    Assert.NotNull(result);
    Assert.NotNull(result.DataModels);
    Assert.NotEmpty(result.DataModels);
  }

  [Fact]
  public async Task GetScenarios_Returns_Any_Scenarios()
  {
    // Arrange: create HttpClient for the in-memory server
    using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions() { BaseAddress = new Uri("http://0.0.0.0:5005") });

    // Create the typed client wrapper
    IAVAIntegrationModelerApiClient apiClient = new AVAIntegrationModelerApiClient(client, new TestHttpClientFactory(client), NullLogger<AVAIntegrationModelerApiClient>.Instance);

    // Act: call the API method
    var result = await apiClient.GetScenarios(Contracts.Datasource.Database, CancellationToken.None);

    // Assert: ensure we got a non-null, non-empty response from the API
    Assert.NotNull(result);
    Assert.NotNull(result.Scenarios);
    Assert.NotEmpty(result.Scenarios);

    // Act: call the API method
    result = await apiClient.GetScenarios(Contracts.Datasource.AVAPlace, CancellationToken.None);

    // Assert: ensure we got a non-null, non-empty response from the API
    Assert.NotNull(result);
    Assert.NotNull(result.Scenarios);
    Assert.NotEmpty(result.Scenarios);
  }

  

  


  [Fact]
  public async Task GetFeatures_Returns_NotNull()
  {
    // Arrange: create HttpClient for the in-memory server
    using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions() { BaseAddress = new Uri("http://0.0.0.0:5005") });

    // Create the typed client wrapper
    IAVAIntegrationModelerApiClient apiClient = new AVAIntegrationModelerApiClient(client, new TestHttpClientFactory(client), NullLogger<AVAIntegrationModelerApiClient>.Instance);

    // Act: call the API method
    var featuresResult = await apiClient.GetFeatures(Contracts.Datasource.AVAPlace, CancellationToken.None);

    //var featureResult = await apiClient.GetFeature(Contracts.Datasource.AVAPlace, scenariosResponse.Features.ElementAt(0).Id, CancellationToken.None);

    Assert.NotNull(featuresResult);
    Assert.NotEmpty(featuresResult.Features);
  }

  [Fact()]
  public async Task CreateScenario_Fail()
  {
    // Arrange: create HttpClient for the in-memory server
    using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions() { BaseAddress = new Uri("http://0.0.0.0:5005") });

    // Create the typed client wrapper
    IAVAIntegrationModelerApiClient apiClient = new AVAIntegrationModelerApiClient(client, new TestHttpClientFactory(client), NullLogger<AVAIntegrationModelerApiClient>.Instance);

    // Act: call the API method
    var scenariosResponse = await apiClient.GetScenarios(Contracts.Datasource.Database, CancellationToken.None);

    
    var createResult = await apiClient.CreateScenario(Contracts.Datasource.Database, scenariosResponse.Scenarios.ElementAt(0), CancellationToken.None);

    Assert.False(createResult.IsSuccess);
    
  }

  [Fact()]
  public async Task CreateExistingScenario_Returns_Error()
  {
    // Arrange
    using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions() 
    { 
      BaseAddress = new Uri("http://0.0.0.0:5005") 
    });

    IAVAIntegrationModelerApiClient apiClient = new AVAIntegrationModelerApiClient(
      client,
      new TestHttpClientFactory(client),
      NullLogger<AVAIntegrationModelerApiClient>.Instance);

    var scenariosResponse = await apiClient.GetScenarios(Contracts.Datasource.Database, CancellationToken.None);
    var existingScenario = scenariosResponse.Scenarios.ElementAt(0);

    // Act - pokus o vytvoření duplicitního scénáře
    var result = await apiClient.CreateScenario(
      Contracts.Datasource.Database, 
      existingScenario, 
      CancellationToken.None);

    // Assert
    Assert.False(result.IsSuccess);
  }

  [Fact()]
  public async Task CreateScenario_WithResult_Success()
  {
    using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions() 
    { 
      BaseAddress = new Uri("http://0.0.0.0:5005") 
    });

    IAVAIntegrationModelerApiClient apiClient = new AVAIntegrationModelerApiClient(
      client,
      new TestHttpClientFactory(client),
      NullLogger<AVAIntegrationModelerApiClient>.Instance);

    var newScenario = new Contracts.DTO.ScenarioDTO
    {
      Id = Guid.NewGuid(),
      Code = $"TEST-{Guid.NewGuid().ToString().Substring(0, 8)}",
      Name = new Contracts.DTO.LocalizedValue 
      { 
        CzechValue = "Test", 
        EnglishValue = "Test" 
      }
    };

    // Act
    var result = await apiClient.CreateScenario(
      Contracts.Datasource.Database, 
      newScenario, 
      CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal(newScenario.Id, result.Value);
  }

  [Fact()]
  public async Task DeleteScenario_WithResult_Success()
  {
    using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
    {
      BaseAddress = new Uri("http://0.0.0.0:5005")
    });

    IAVAIntegrationModelerApiClient apiClient = new AVAIntegrationModelerApiClient(
      client,
      new TestHttpClientFactory(client),
      NullLogger<AVAIntegrationModelerApiClient>.Instance);

    var newScenario = new Contracts.DTO.ScenarioDTO
    {
      Id = Guid.NewGuid(),
      Code = $"TEST-{Guid.NewGuid().ToString().Substring(0, 8)}",
      Name = new Contracts.DTO.LocalizedValue
      {
        CzechValue = "Test",
        EnglishValue = "Test"
      }
    };

    // Act
    _ =  await apiClient.CreateScenario(
      Contracts.Datasource.Database,
      newScenario,
      CancellationToken.None);

    var deleteResult = await apiClient.DeleteScenario(
      Contracts.Datasource.Database, newScenario.Code, CancellationToken.None);

    Assert.True(deleteResult.IsNoContent());  


  }
}
