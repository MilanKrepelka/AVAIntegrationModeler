using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.ValidationServices;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.ValidationServices;

/// <summary>
/// Testy pro DataModelValidationService — ověřují unikátnost kódu datového modelu
/// při vytvoření i aktualizaci.
/// </summary>
public class DataModelValidationServiceTests : IAsyncLifetime, IAsyncDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly EfRepository<DataModel> _repository;
  private readonly DataModelValidationService _sut;

  public DataModelValidationServiceTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: $"DataModelValidationTestDb_{Guid.NewGuid():N}")
        .EnableSensitiveDataLogging()
        .Options;

    _dbContext = new AppDbContext(options, null);
    _repository = new EfRepository<DataModel>(_dbContext);
    _sut = new DataModelValidationService(_repository);
  }

  public async ValueTask InitializeAsync() => await _dbContext.Database.EnsureCreatedAsync();

  public async ValueTask DisposeAsync()
  {
    await _dbContext.Database.EnsureDeletedAsync();
    await _dbContext.DisposeAsync();
  }

  private async Task<DataModel> SeedModelAsync(string code)
  {
    var model = new DataModel(Guid.NewGuid(), code).SetName(code);
    await _repository.AddAsync(model, CancellationToken.None);
    return model;
  }

  // ---------------------------------------------------------------
  // ValidateForCreate
  // ---------------------------------------------------------------

  [Fact]
  public async Task ValidateForCreate_UniqueCodeAndId_ReturnsSuccess()
  {
    var model = new DataModel(Guid.NewGuid(), "CUSTOMER").SetName("Customer");

    var result = await _sut.ValidateForCreate(Datasource.Database, model, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateCode_ReturnsConflict()
  {
    await SeedModelAsync("CUSTOMER");
    var duplicate = new DataModel(Guid.NewGuid(), "CUSTOMER").SetName("Customer 2");

    var result = await _sut.ValidateForCreate(Datasource.Database, duplicate, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Conflict);
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateId_ReturnsInvalid()
  {
    var existing = await SeedModelAsync("CUSTOMER");
    var duplicate = new DataModel(existing.Id, "ORDER").SetName("Order");

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
    var existing = await SeedModelAsync("CUSTOMER");
    existing.SetCode("CUSTOMER-NEW");

    var result = await _sut.Validate(Datasource.Database, existing, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task Validate_RenameToCodeUsedByAnotherModel_ReturnsConflict()
  {
    await SeedModelAsync("ORDER");
    var toRename = await SeedModelAsync("CUSTOMER");
    toRename.SetCode("ORDER");

    var result = await _sut.Validate(Datasource.Database, toRename, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Conflict);
  }
}
