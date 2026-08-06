using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AVAIntegrationModeler.API.Client.Test;

public class DataModelClientTests : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private readonly AVAIntegrationModelerAPIFactory _factory;

  public DataModelClientTests(AVAIntegrationModelerAPIFactory factory)
  {
    _factory = factory;
  }

  private IAVAIntegrationModelerApiClient CreateClient()
  {
    var http = _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      BaseAddress = new Uri("http://0.0.0.0:5005")
    });
    return new AVAIntegrationModelerApiClient(http, new TestHttpClientFactory(http), NullLogger<AVAIntegrationModelerApiClient>.Instance);
  }

  private static DataModelDTO NewDataModel(string? code = null) => new DataModelDTO
  {
    Id = Guid.NewGuid(),
    Code = code ?? $"TEST-{Guid.NewGuid().ToString()[..8].ToUpper()}",
    Name = "Test DataModel",
    Description = "Popis testovacího modelu",
    Notes = "Poznámky",
    IsAggregateRoot = true,
    Fields = []
  };

  [Fact]
  public async Task CreateDataModel_WithValidData_ReturnsSuccessWithGuid()
  {
    var client = CreateClient();
    var dm = NewDataModel();

    var result = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.NotEqual(Guid.Empty, result.Value);
  }

  [Fact]
  public async Task CreateDataModel_IdIsPreservedFromRequest()
  {
    var client = CreateClient();
    var dm = NewDataModel();

    var result = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);

    Assert.True(result.IsSuccess);
    Assert.Equal(dm.Id, result.Value);
  }

  [Fact]
  public async Task CreateDataModel_WithDuplicateCode_ReturnsError()
  {
    var client = CreateClient();
    var dm = NewDataModel();

    await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);

    var duplicate = dm with { Id = Guid.NewGuid() };
    var result = await client.CreateDataModel(Datasource.Database, duplicate, CancellationToken.None);

    Assert.False(result.IsSuccess);
  }

  [Fact]
  public async Task CreateDataModel_WithEmptyCode_ReturnsInvalid()
  {
    var client = CreateClient();
    var dm = NewDataModel() with { Code = string.Empty };

    var result = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(ResultStatus.Invalid, result.Status);
  }

  [Fact]
  public async Task CreateDataModel_WithEmptyName_ReturnsInvalid()
  {
    var client = CreateClient();
    var dm = NewDataModel() with { Name = string.Empty };

    var result = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(ResultStatus.Invalid, result.Status);
  }

  [Fact]
  public async Task GetDataModel_AfterCreate_ReturnsCorrectData()
  {
    var client = CreateClient();
    var dm = NewDataModel();

    var createResult = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(createResult.IsSuccess);

    var fetched = await client.GetDataModel(Datasource.Database, createResult.Value, CancellationToken.None);

    Assert.NotNull(fetched);
    Assert.Equal(dm.Code, fetched.Code);
    Assert.Equal(dm.Name, fetched.Name);
    Assert.Equal(dm.IsAggregateRoot, fetched.IsAggregateRoot);
  }

  [Fact]
  public async Task UpdateDataModel_WithValidData_ReturnsSuccess()
  {
    var client = CreateClient();
    var dm = NewDataModel();

    var createResult = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(createResult.IsSuccess);

    var updated = dm with { Id = createResult.Value, Name = "Aktualizovaný název", Description = "Nový popis" };

    var updateResult = await client.UpdateDataModel(Datasource.Database, updated, CancellationToken.None);

    Assert.True(updateResult.IsSuccess);
  }

  [Fact]
  public async Task UpdateDataModel_NonExistentId_ReturnsNotFound()
  {
    var client = CreateClient();
    var dm = NewDataModel() with { Id = Guid.NewGuid() };

    var result = await client.UpdateDataModel(Datasource.Database, dm, CancellationToken.None);

    Assert.False(result.IsSuccess);
    Assert.Equal(ResultStatus.NotFound, result.Status);
  }

  [Fact]
  public async Task DeleteDataModel_AfterCreate_ReturnsNoContent()
  {
    var client = CreateClient();
    var dm = NewDataModel();

    var createResult = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(createResult.IsSuccess);

    var deleteResult = await client.DeleteDataModel(Datasource.Database, createResult.Value, CancellationToken.None);

    Assert.True(deleteResult.IsNoContent());
  }

  [Fact]
  public async Task DeleteDataModel_NonExistentId_ReturnsError()
  {
    var client = CreateClient();

    var result = await client.DeleteDataModel(Datasource.Database, Guid.NewGuid(), CancellationToken.None);

    Assert.False(result.IsSuccess);
  }

  [Fact]
  public async Task CreateUpdateDelete_FullLifecycle_Works()
  {
    var client = CreateClient();
    var dm = NewDataModel();

    var createResult = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(createResult.IsSuccess);
    var id = createResult.Value;

    var updated = dm with { Id = id, Name = "Upravený název" };
    var updateResult = await client.UpdateDataModel(Datasource.Database, updated, CancellationToken.None);
    Assert.True(updateResult.IsSuccess);

    var fetched = await client.GetDataModel(Datasource.Database, id, CancellationToken.None);
    Assert.Equal("Upravený název", fetched.Name);

    var deleteResult = await client.DeleteDataModel(Datasource.Database, id, CancellationToken.None);
    Assert.True(deleteResult.IsNoContent());
  }
}
