using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Test.Data.Queries;

/// <summary>
/// Testy pro AreasQueryService — testování metod pro načítání oblastí z databáze.
/// </summary>
public class AreasQueryServiceTests : IAsyncLifetime, IAsyncDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly IMemoryCache _memoryCache;
  private readonly AreasQueryService _sut;

  public AreasQueryServiceTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: $"AreasTestDb_{Guid.NewGuid():N}")
        .EnableSensitiveDataLogging()
        .Options;

    _dbContext = new AppDbContext(options, null);
    _memoryCache = new MemoryCache(new MemoryCacheOptions());

    _sut = new AreasQueryService(_dbContext, _memoryCache);
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

  // ---------------------------------------------------------------
  // ListAsync
  // ---------------------------------------------------------------

  [Fact]
  public async Task ListAsync_EmptyDatabase_ReturnsEmptyList()
  {
    var result = await _sut.ListAsync(Datasource.Database);

    Assert.NotNull(result);
    Assert.Empty(result);
  }

  [Fact]
  public async Task ListAsync_WithAreas_ReturnsAllAreas()
  {
    var a1 = new Area(Guid.NewGuid(), "AREA-A").SetName("Area A");
    var a2 = new Area(Guid.NewGuid(), "AREA-B").SetName("Area B");
    var a3 = new Area(Guid.NewGuid(), "AREA-C").SetName("Area C");
    _dbContext.Areas.AddRange(a1, a2, a3);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = (await _sut.ListAsync(Datasource.Database)).ToList();

    Assert.Equal(3, result.Count);
    Assert.Contains(result, a => a.Code == "AREA-A");
    Assert.Contains(result, a => a.Code == "AREA-B");
    Assert.Contains(result, a => a.Code == "AREA-C");
  }

  [Fact]
  public async Task ListAsync_UsesCaching_SecondCallReturnsSameData()
  {
    var area = new Area(Guid.NewGuid(), "CACHED-AREA").SetName("Cached");
    _dbContext.Areas.Add(area);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    // First call — populates cache
    var result1 = (await _sut.ListAsync(Datasource.Database)).ToList();

    // Add second area without invalidating cache
    var area2 = new Area(Guid.NewGuid(), "NEW-AREA").SetName("New");
    _dbContext.Areas.Add(area2);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    // Second call — should return cached result (1 item)
    var result2 = (await _sut.ListAsync(Datasource.Database)).ToList();

    Assert.Single(result1);
    Assert.Single(result2);
    Assert.Equal(result1[0].Code, result2[0].Code);
  }

  [Fact]
  public async Task InvalidateCache_ThenList_ReturnsUpdatedData()
  {
    var area = new Area(Guid.NewGuid(), "FIRST-AREA").SetName("First");
    _dbContext.Areas.Add(area);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    // First call — caches 1 item
    var before = (await _sut.ListAsync(Datasource.Database)).ToList();
    Assert.Single(before);

    // Add second area
    var area2 = new Area(Guid.NewGuid(), "SECOND-AREA").SetName("Second");
    _dbContext.Areas.Add(area2);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    // Invalidate and reload
    _sut.InvalidateCache(Datasource.Database);
    var after = (await _sut.ListAsync(Datasource.Database)).ToList();

    Assert.Equal(2, after.Count);
    Assert.Contains(after, a => a.Code == "SECOND-AREA");
  }

  // ---------------------------------------------------------------
  // GetArea by Id
  // ---------------------------------------------------------------

  [Fact]
  public async Task GetArea_ById_ExistingArea_ReturnsCorrectArea()
  {
    var id = Guid.NewGuid();
    var area = new Area(id, "GET-BY-ID").SetName("Get By Id Area");
    _dbContext.Areas.Add(area);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = await _sut.GetArea(Datasource.Database, id, TestContext.Current.CancellationToken);

    Assert.NotNull(result);
    Assert.Equal(id, result.Id);
    Assert.Equal("GET-BY-ID", result.Code);
    Assert.Equal("Get By Id Area", result.Name);
  }

  [Fact]
  public async Task GetArea_ById_NonExistingArea_ThrowsNotFoundException()
  {
    await Assert.ThrowsAsync<NotFoundException>(async () =>
      await _sut.GetArea(Datasource.Database, Guid.NewGuid(), TestContext.Current.CancellationToken));
  }

  // ---------------------------------------------------------------
  // GetArea by Code
  // ---------------------------------------------------------------

  [Fact]
  public async Task GetArea_ByCode_ExistingArea_ReturnsCorrectArea()
  {
    var id = Guid.NewGuid();
    var area = new Area(id, "GET-BY-CODE").SetName("Get By Code Area");
    _dbContext.Areas.Add(area);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = await _sut.GetArea(Datasource.Database, "GET-BY-CODE", TestContext.Current.CancellationToken);

    Assert.NotNull(result);
    Assert.Equal(id, result.Id);
    Assert.Equal("Get By Code Area", result.Name);
  }

  [Fact]
  public async Task GetArea_ByCode_NonExistingArea_ThrowsNotFoundException()
  {
    await Assert.ThrowsAsync<NotFoundException>(async () =>
      await _sut.GetArea(Datasource.Database, "NONEXISTENT", TestContext.Current.CancellationToken));
  }

  // ---------------------------------------------------------------
  // ExistsByIdAsync
  // ---------------------------------------------------------------

  [Fact]
  public async Task ExistsByIdAsync_ExistingArea_ReturnsTrue()
  {
    var id = Guid.NewGuid();
    var area = new Area(id, "EXISTS-ID").SetName("Exists");
    _dbContext.Areas.Add(area);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = await _sut.ExistsByIdAsync(Datasource.Database, id, TestContext.Current.CancellationToken);

    Assert.True(result);
  }

  [Fact]
  public async Task ExistsByIdAsync_NonExistingArea_ReturnsFalse()
  {
    var result = await _sut.ExistsByIdAsync(Datasource.Database, Guid.NewGuid(), TestContext.Current.CancellationToken);

    Assert.False(result);
  }

  // ---------------------------------------------------------------
  // ExistsByCodeAsync
  // ---------------------------------------------------------------

  [Fact]
  public async Task ExistsByCodeAsync_ExistingArea_ReturnsTrue()
  {
    var area = new Area(Guid.NewGuid(), "EXISTS-CODE").SetName("Exists");
    _dbContext.Areas.Add(area);
    await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var result = await _sut.ExistsByCodeAsync(Datasource.Database, "EXISTS-CODE", TestContext.Current.CancellationToken);

    Assert.True(result);
  }

  [Fact]
  public async Task ExistsByCodeAsync_NonExistingArea_ReturnsFalse()
  {
    var result = await _sut.ExistsByCodeAsync(Datasource.Database, "MISSING", TestContext.Current.CancellationToken);

    Assert.False(result);
  }
}
