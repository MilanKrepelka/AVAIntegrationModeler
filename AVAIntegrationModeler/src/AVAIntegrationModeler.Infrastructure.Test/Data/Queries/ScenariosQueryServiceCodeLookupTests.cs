using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.Data.Queries;

/// <summary>
/// Testy pro ScenariosQueryService — ověřují, že vyhledání scénáře podle kódu
/// (ExistsByCodeAsync, GetScenario) je pro Datasource.Database case-insensitive.
/// </summary>
public class ScenariosQueryServiceCodeLookupTests : IAsyncLifetime, IAsyncDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly IMemoryCache _memoryCache;
  private readonly ScenariosQueryService _sut;

  public ScenariosQueryServiceCodeLookupTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: $"ScenariosCodeLookupTestDb_{Guid.NewGuid():N}")
        .EnableSensitiveDataLogging()
        .Options;

    _dbContext = new AppDbContext(options, null);
    _memoryCache = new MemoryCache(new MemoryCacheOptions());
    _sut = new ScenariosQueryService(_dbContext, Substitute.For<IIntegrationDataProvider>(), _memoryCache);
  }

  public async ValueTask InitializeAsync() => await _dbContext.Database.EnsureCreatedAsync();

  public async ValueTask DisposeAsync()
  {
    await _dbContext.Database.EnsureDeletedAsync();
    await _dbContext.DisposeAsync();
    _memoryCache.Dispose();
  }

  private async Task SeedScenarioAsync(string code)
  {
    var scenario = new Scenario(Guid.NewGuid()).SetCode(code);
    _dbContext.Scenarios.Add(scenario);
    await _dbContext.SaveChangesAsync();
  }

  [Fact]
  public async Task ExistsByCodeAsync_DifferentCase_ReturnsTrue()
  {
    await SeedScenarioAsync("SC-1");

    var exists = await _sut.ExistsByCodeAsync(Datasource.Database, "sc-1", CancellationToken.None);

    exists.ShouldBeTrue();
  }

  [Fact]
  public async Task GetScenario_ByCodeDifferentCase_ReturnsScenario()
  {
    await SeedScenarioAsync("SC-1");

    var result = await _sut.GetScenario(Datasource.Database, "sc-1", CancellationToken.None);

    result.ShouldNotBeNull();
    result.Code.ShouldBe("SC-1");
  }
}
