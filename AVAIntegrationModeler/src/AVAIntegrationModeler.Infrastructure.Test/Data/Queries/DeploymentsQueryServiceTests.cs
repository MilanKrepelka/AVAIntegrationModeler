using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Test.Data.Queries;

/// <summary>
/// Testy pro DeploymentsQueryService.
/// </summary>
public class DeploymentsQueryServiceTests : IAsyncLifetime, IAsyncDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly IMemoryCache _memoryCache;
  private readonly DeploymentsQueryService _sut;

  public DeploymentsQueryServiceTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
      .UseInMemoryDatabase(databaseName: $"DeploymentsTestDb_{Guid.NewGuid():N}")
      .EnableSensitiveDataLogging()
      .Options;

    _dbContext = new AppDbContext(options, null);
    _memoryCache = new MemoryCache(new MemoryCacheOptions());
    _sut = new DeploymentsQueryService(_dbContext, _memoryCache);
  }

  public async ValueTask InitializeAsync()
  {
    await _dbContext.Database.EnsureCreatedAsync();
  }

  public async ValueTask DisposeAsync()
  {
    await _dbContext.Database.EnsureDeletedAsync();
    await _dbContext.DisposeAsync();
    _memoryCache?.Dispose();
  }

  [Fact]
  public async Task ListAsync_PrazdnaDB_VratiPrazdnySeznam()
  {
    var result = await _sut.ListAsync();
    Assert.NotNull(result);
    Assert.Empty(result);
  }

  [Fact]
  public async Task ListAsync_SNasazenimi_VratiVse()
  {
    var d1 = new Deployment(Guid.NewGuid(), "DEP-A"); d1.SetName("A");
    var d2 = new Deployment(Guid.NewGuid(), "DEP-B"); d2.SetName("B");
    _dbContext.Deployments.AddRange(d1, d2);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = (await _sut.ListAsync()).ToList();

    Assert.Equal(2, result.Count);
    Assert.Contains(result, d => d.Code == "DEP-A");
    Assert.Contains(result, d => d.Code == "DEP-B");
  }

  [Fact]
  public async Task ListAsync_VraciDataModely()
  {
    var modelId = Guid.NewGuid();
    var dep = new Deployment(Guid.NewGuid(), "DEP-DM"); dep.SetName("With DM");
    dep.AddDataModel(modelId);
    _dbContext.Deployments.Add(dep);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = (await _sut.ListAsync()).ToList();

    Assert.Single(result);
    Assert.Contains(modelId, result[0].DataModelIds);
  }

  [Fact]
  public async Task ListAsync_UsesCaching_DruhyVolaniVraciCache()
  {
    var dep = new Deployment(Guid.NewGuid(), "CACHE-DEP"); dep.SetName("Cached");
    _dbContext.Deployments.Add(dep);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result1 = (await _sut.ListAsync()).ToList();

    var dep2 = new Deployment(Guid.NewGuid(), "CACHE-DEP-2"); dep2.SetName("New");
    _dbContext.Deployments.Add(dep2);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result2 = (await _sut.ListAsync()).ToList();

    Assert.Single(result1);
    Assert.Single(result2);
  }

  [Fact]
  public async Task InvalidateCache_ThenList_VratiAktualniData()
  {
    var dep = new Deployment(Guid.NewGuid(), "INV-DEP"); dep.SetName("First");
    _dbContext.Deployments.Add(dep);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var before = (await _sut.ListAsync()).ToList();
    Assert.Single(before);

    var dep2 = new Deployment(Guid.NewGuid(), "INV-DEP-2"); dep2.SetName("Second");
    _dbContext.Deployments.Add(dep2);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    _sut.InvalidateCache(Datasource.Database);
    var after = (await _sut.ListAsync()).ToList();

    Assert.Equal(2, after.Count);
  }

  [Fact]
  public async Task GetDeployment_ById_VratiSpravny()
  {
    var id = Guid.NewGuid();
    var dep = new Deployment(id, "GET-ID"); dep.SetName("By Id");
    _dbContext.Deployments.Add(dep);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = await _sut.GetDeployment(id, TestContext.Current.CancellationToken);

    Assert.NotNull(result);
    Assert.Equal(id, result.Id);
    Assert.Equal("GET-ID", result.Code);
  }

  [Fact]
  public async Task GetDeployment_ById_Neexistujici_VyhodiNotFoundException()
  {
    await Assert.ThrowsAsync<NotFoundException>(async () =>
      await _sut.GetDeployment(Guid.NewGuid(), TestContext.Current.CancellationToken));
  }

  [Fact]
  public async Task GetDeployment_ByCode_VratiSpravny()
  {
    var dep = new Deployment(Guid.NewGuid(), "GET-CODE"); dep.SetName("By Code");
    _dbContext.Deployments.Add(dep);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = await _sut.GetDeployment("GET-CODE", TestContext.Current.CancellationToken);

    Assert.NotNull(result);
    Assert.Equal("GET-CODE", result.Code);
  }

  [Fact]
  public async Task GetDeployment_ByCode_Neexistujici_VyhodiNotFoundException()
  {
    await Assert.ThrowsAsync<NotFoundException>(async () =>
      await _sut.GetDeployment("MISSING", TestContext.Current.CancellationToken));
  }

  [Fact]
  public async Task ExistsByCodeAsync_Existujici_VratiTrue()
  {
    var dep = new Deployment(Guid.NewGuid(), "EXISTS"); dep.SetName("E");
    _dbContext.Deployments.Add(dep);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = await _sut.ExistsByCodeAsync("EXISTS", TestContext.Current.CancellationToken);
    Assert.True(result);
  }

  [Fact]
  public async Task ExistsByCodeAsync_Neexistujici_VratiFalse()
  {
    var result = await _sut.ExistsByCodeAsync("NOPE", TestContext.Current.CancellationToken);
    Assert.False(result);
  }
}
