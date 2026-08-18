using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.ValidationServices;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.ValidationServices;

/// <summary>
/// Testy pro DeploymentValidationService — ověřují unikátnost kódu nasazení
/// při vytvoření i aktualizaci.
/// </summary>
public class DeploymentValidationServiceTests : IAsyncLifetime, IAsyncDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly EfRepository<Deployment> _repository;
  private readonly DeploymentValidationService _sut;

  public DeploymentValidationServiceTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: $"DeploymentValidationTestDb_{Guid.NewGuid():N}")
        .EnableSensitiveDataLogging()
        .Options;

    _dbContext = new AppDbContext(options, null);
    _repository = new EfRepository<Deployment>(_dbContext);
    _sut = new DeploymentValidationService(_repository);
  }

  public async ValueTask InitializeAsync() => await _dbContext.Database.EnsureCreatedAsync();

  public async ValueTask DisposeAsync()
  {
    await _dbContext.Database.EnsureDeletedAsync();
    await _dbContext.DisposeAsync();
  }

  private async Task<Deployment> SeedDeploymentAsync(string code)
  {
    var deployment = new Deployment(Guid.NewGuid(), code).SetName(code);
    await _repository.AddAsync(deployment, CancellationToken.None);
    return deployment;
  }

  // ---------------------------------------------------------------
  // ValidateForCreate
  // ---------------------------------------------------------------

  [Fact]
  public async Task ValidateForCreate_UniqueCodeAndId_ReturnsSuccess()
  {
    var deployment = new Deployment(Guid.NewGuid(), "DEP-1").SetName("Deployment 1");

    var result = await _sut.ValidateForCreate(Datasource.Database, deployment, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateCode_ReturnsConflict()
  {
    await SeedDeploymentAsync("DEP-1");
    var duplicate = new Deployment(Guid.NewGuid(), "DEP-1").SetName("Deployment 1 duplicate");

    var result = await _sut.ValidateForCreate(Datasource.Database, duplicate, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Conflict);
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateCodeDifferentCase_ReturnsConflict()
  {
    await SeedDeploymentAsync("DEP-1");
    var duplicate = new Deployment(Guid.NewGuid(), "dep-1").SetName("Deployment 1 lowercase");

    var result = await _sut.ValidateForCreate(Datasource.Database, duplicate, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Conflict);
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateId_ReturnsInvalid()
  {
    var existing = await SeedDeploymentAsync("DEP-1");
    var duplicate = new Deployment(existing.Id, "DEP-2").SetName("Deployment 2");

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
    var existing = await SeedDeploymentAsync("DEP-1");
    existing.SetCode("DEP-1-NEW");

    var result = await _sut.Validate(Datasource.Database, existing, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task Validate_RenameToCodeUsedByAnotherDeployment_ReturnsConflict()
  {
    await SeedDeploymentAsync("DEP-2");
    var toRename = await SeedDeploymentAsync("DEP-1");
    toRename.SetCode("DEP-2");

    var result = await _sut.Validate(Datasource.Database, toRename, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Conflict);
  }

  [Fact]
  public async Task Validate_KeepingOwnCodeWithDifferentCase_ReturnsSuccess()
  {
    var existing = await SeedDeploymentAsync("DEP-1");
    existing.SetCode("dep-1");

    var result = await _sut.Validate(Datasource.Database, existing, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }
}
