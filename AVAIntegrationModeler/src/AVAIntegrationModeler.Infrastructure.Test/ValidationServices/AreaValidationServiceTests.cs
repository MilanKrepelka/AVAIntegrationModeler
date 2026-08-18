using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.ValidationServices;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.ValidationServices;

/// <summary>
/// Testy pro AreaValidationService — ověřují unikátnost kódu oblasti při vytvoření i aktualizaci.
/// </summary>
public class AreaValidationServiceTests : IAsyncLifetime, IAsyncDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly EfRepository<Area> _repository;
  private readonly AreaValidationService _sut;

  public AreaValidationServiceTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: $"AreaValidationTestDb_{Guid.NewGuid():N}")
        .EnableSensitiveDataLogging()
        .Options;

    _dbContext = new AppDbContext(options, null);
    _repository = new EfRepository<Area>(_dbContext);
    _sut = new AreaValidationService(_repository);
  }

  public async ValueTask InitializeAsync() => await _dbContext.Database.EnsureCreatedAsync();

  public async ValueTask DisposeAsync()
  {
    await _dbContext.Database.EnsureDeletedAsync();
    await _dbContext.DisposeAsync();
  }

  private async Task<Area> SeedAreaAsync(string code)
  {
    var area = new Area(Guid.NewGuid(), code).SetName(code);
    await _repository.AddAsync(area, CancellationToken.None);
    return area;
  }

  // ---------------------------------------------------------------
  // ValidateForCreate
  // ---------------------------------------------------------------

  [Fact]
  public async Task ValidateForCreate_UniqueCodeAndId_ReturnsSuccess()
  {
    var area = new Area(Guid.NewGuid(), "UNIQUE").SetName("Unique");

    var result = await _sut.ValidateForCreate(Datasource.Database, area, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateCode_ReturnsConflict()
  {
    await SeedAreaAsync("SALES");
    var duplicate = new Area(Guid.NewGuid(), "SALES").SetName("Sales 2");

    var result = await _sut.ValidateForCreate(Datasource.Database, duplicate, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Conflict);
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateCodeDifferentCase_ReturnsConflict()
  {
    await SeedAreaAsync("SALES");
    var duplicate = new Area(Guid.NewGuid(), "sales").SetName("Sales lowercase");

    var result = await _sut.ValidateForCreate(Datasource.Database, duplicate, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Conflict);
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateId_ReturnsInvalid()
  {
    var existing = await SeedAreaAsync("SALES");
    var duplicate = new Area(existing.Id, "DIFFERENT-CODE").SetName("Different");

    var result = await _sut.ValidateForCreate(Datasource.Database, duplicate, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Invalid);
  }

  // ---------------------------------------------------------------
  // Validate (update)
  // ---------------------------------------------------------------

  [Fact]
  public async Task Validate_RenameToUnusedCode_ReturnsSuccess()
  {
    var existing = await SeedAreaAsync("SALES");
    existing.SetCode("SALES-NEW");

    var result = await _sut.Validate(Datasource.Database, existing, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task Validate_KeepingOwnCode_ReturnsSuccess()
  {
    var existing = await SeedAreaAsync("SALES");

    var result = await _sut.Validate(Datasource.Database, existing, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task Validate_RenameToCodeUsedByAnotherArea_ReturnsConflict()
  {
    await SeedAreaAsync("FINANCE");
    var toRename = await SeedAreaAsync("SALES");
    toRename.SetCode("FINANCE");

    var result = await _sut.Validate(Datasource.Database, toRename, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Conflict);
  }

  [Fact]
  public async Task Validate_RenameToCodeUsedByAnotherArea_DifferentCase_ReturnsConflict()
  {
    await SeedAreaAsync("FINANCE");
    var toRename = await SeedAreaAsync("SALES");
    toRename.SetCode("finance");

    var result = await _sut.Validate(Datasource.Database, toRename, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Conflict);
  }

  [Fact]
  public async Task Validate_KeepingOwnCodeWithDifferentCase_ReturnsSuccess()
  {
    var existing = await SeedAreaAsync("SALES");
    existing.SetCode("sales");

    var result = await _sut.Validate(Datasource.Database, existing, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }
}
