using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.FeatureAggregate;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace AVAIntegrationModeler.Infrastructure.Test.Data.Queries;

public class ListScenariosQueryServiceTests : IAsyncLifetime, IAsyncDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly IIntegrationDataProvider _mockIntegrationDataProvider;
  private readonly IMemoryCache _memoryCache;
  private readonly ListScenariosQueryService _sut;
  private readonly string _databaseName;

  public ListScenariosQueryServiceTests()
  {
    _databaseName = $"TestDb_{Guid.NewGuid():N}";

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: _databaseName)
        .EnableSensitiveDataLogging() // Helpful for debugging
        .Options;

    _dbContext = new AppDbContext(options, null);
    _mockIntegrationDataProvider = Substitute.For<IIntegrationDataProvider>();
    _memoryCache = new MemoryCache(new MemoryCacheOptions());

    _sut = new ListScenariosQueryService(
        _dbContext,
        _mockIntegrationDataProvider,
        _memoryCache);
  }

  public async ValueTask InitializeAsync()
  {
    // Ensure database is created and ready
    await _dbContext.Database.EnsureCreatedAsync();
  }

  public async ValueTask DisposeAsync()
  {
    // Clean up resources
    await _dbContext.Database.EnsureDeletedAsync();
    await _dbContext.DisposeAsync();
    _memoryCache?.Dispose();
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_ReturnsEmptyList_WhenNoScenarios()
  {
    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_ReturnsScenarios_WhenScenariosExist()
  {
    // Arrange
    var scenarioId = Guid.NewGuid();
    var scenario = new Scenario(scenarioId);
    scenario.SetName(new LocalizedValue
    {
      CzechValue = "Testovací scénář",
      EnglishValue = "Test Scenario"
    });
    scenario.SetDescription(new LocalizedValue
    {
      CzechValue = "Popis testovacího scénáře",
      EnglishValue = "Test scenario description"
    });

    var codeProperty = typeof(Scenario).GetProperty("Code");
    codeProperty!.SetValue(scenario, "TEST-001");

    _dbContext.Scenarios.Add(scenario);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    Assert.NotNull(result);
    var scenarios = result.ToList();
    Assert.Single(scenarios);
    Assert.Equal("TEST-001", scenarios[0].Code);
    Assert.Equal(scenarioId, scenarios[0].Id);
    Assert.Equal("Testovací scénář", scenarios[0].Name.CzechValue);
    Assert.Equal("Test Scenario", scenarios[0].Name.EnglishValue);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_ReturnsScenariosWithFeatures_WhenInputAndOutputFeaturesAreSet()
  {
    // Arrange
    var inputFeatureId = Guid.NewGuid();
    var outputFeatureId = Guid.NewGuid();

    var inputFeature = new Feature(inputFeatureId, "INPUT-FEATURE-001");
    inputFeature.SetName(new LocalizedValue
    {
      CzechValue = "Vstupní feature",
      EnglishValue = "Input Feature"
    });

    var outputFeature = new Feature(outputFeatureId, "OUTPUT-FEATURE-001");
    outputFeature.SetName(new LocalizedValue
    {
      CzechValue = "Výstupní feature",
      EnglishValue = "Output Feature"
    });

    _dbContext.Features.AddRange(inputFeature, outputFeature);

    var scenario = new Scenario(Guid.NewGuid());
    var codeProperty = typeof(Scenario).GetProperty("Code");
    codeProperty!.SetValue(scenario, "SCEN-001");

    scenario.SetName(new LocalizedValue
    {
      CzechValue = "Scénář s features",
      EnglishValue = "Scenario with features"
    });
    scenario.SetInputFeature(inputFeatureId);
    scenario.SetOutputFeature(outputFeatureId);

    _dbContext.Scenarios.Add(scenario);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    var scenarios = result.ToList();
    Assert.Single(scenarios);

    var firstScenario = scenarios[0];
    Assert.Equal(inputFeatureId, firstScenario.InputFeatureId);
    Assert.Equal(outputFeatureId, firstScenario.OutputFeatureId);

    Assert.NotNull(firstScenario.InputFeatureSummary);
    Assert.Equal(inputFeatureId, firstScenario.InputFeatureSummary.Id);
    Assert.Equal("INPUT-FEATURE-001", firstScenario.InputFeatureSummary.Code);

    Assert.NotNull(firstScenario.OutputFeatureSummary);
    Assert.Equal(outputFeatureId, firstScenario.OutputFeatureSummary.Id);
    Assert.Equal("OUTPUT-FEATURE-001", firstScenario.OutputFeatureSummary.Code);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_ReturnsScenarioWithoutFeatures_WhenNoInputOrOutputIsSet()
  {
    // Arrange
    var scenario = new Scenario(Guid.NewGuid());
    var codeProperty = typeof(Scenario).GetProperty("Code");
    codeProperty!.SetValue(scenario, "SCEN-NO-FEATURES");

    scenario.SetName(new LocalizedValue
    {
      CzechValue = "Scénář bez features",
      EnglishValue = "Scenario without features"
    });

    _dbContext.Scenarios.Add(scenario);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    var scenarios = result.ToList();
    Assert.Single(scenarios);

    var firstScenario = scenarios[0];
    Assert.Null(firstScenario.InputFeatureId);
    Assert.Null(firstScenario.OutputFeatureId);
    Assert.Null(firstScenario.InputFeatureSummary);
    Assert.Null(firstScenario.OutputFeatureSummary);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_ReturnsMultipleScenarios()
  {
    // Arrange
    // ✅ ZMĚNA: Použít unikátní kódy pro každý běh testu
    var scenario1 = new Scenario(Guid.NewGuid());
    var codeProperty = typeof(Scenario).GetProperty("Code");
    var uniqueCode1 = $"SCEN-001-{Guid.NewGuid():N}";
    codeProperty!.SetValue(scenario1, uniqueCode1);
    scenario1.SetName(new LocalizedValue { CzechValue = "První", EnglishValue = "First" });

    var scenario2 = new Scenario(Guid.NewGuid());
    var uniqueCode2 = $"SCEN-002-{Guid.NewGuid():N}";
    codeProperty.SetValue(scenario2, uniqueCode2);
    scenario2.SetName(new LocalizedValue { CzechValue = "Druhý", EnglishValue = "Second" });

    var scenario3 = new Scenario(Guid.NewGuid());
    var uniqueCode3 = $"SCEN-003-{Guid.NewGuid():N}";
    codeProperty.SetValue(scenario3, uniqueCode3);
    scenario3.SetName(new LocalizedValue { CzechValue = "Třetí", EnglishValue = "Third" });

    _dbContext.Scenarios.AddRange(scenario1, scenario2, scenario3);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    var scenarios = result.ToList();
    Assert.Equal(3, scenarios.Count);
    Assert.Contains(scenarios, s => s.Code == uniqueCode1);
    Assert.Contains(scenarios, s => s.Code == uniqueCode2);
    Assert.Contains(scenarios, s => s.Code == uniqueCode3);
  }

  [Fact]
  public async Task ListAsync_WithAVAPlaceDatasource_CallsIntegrationProvider()
  {
    // Arrange
    var expectedScenarios = new List<Contracts.DTO.ScenarioDTO>
    {
      new Contracts.DTO.ScenarioDTO
      {
        Id = Guid.NewGuid(),
        Code = "AVA-001",
        Name = new LocalizedValue
        {
          CzechValue = "AVA scénář",
          EnglishValue = "AVA Scenario"
        },
        Description = new LocalizedValue
        {
          CzechValue = "Popis z AVAPlace",
          EnglishValue = "Description from AVAPlace"
        }
      }
    };

    _mockIntegrationDataProvider
        .GetScenariosAsync(Arg.Any<CancellationToken>())
        .Returns(expectedScenarios);

    // Act
    var result = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    Assert.NotNull(result);
    var scenarios = result.ToList();
    Assert.Single(scenarios);
    Assert.Equal("AVA-001", scenarios[0].Code);
    Assert.Equal("AVA scénář", scenarios[0].Name.CzechValue);

    await _mockIntegrationDataProvider
        .Received(1)
        .GetScenariosAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ListAsync_WithAVAPlaceDatasource_ReturnsMultipleScenarios()
  {
    // Arrange
    var expectedScenarios = new List<Contracts.DTO.ScenarioDTO>
    {
      new Contracts.DTO.ScenarioDTO { Id = Guid.NewGuid(), Code = "AVA-001" },
      new Contracts.DTO.ScenarioDTO { Id = Guid.NewGuid(), Code = "AVA-002" },
      new Contracts.DTO.ScenarioDTO { Id = Guid.NewGuid(), Code = "AVA-003" }
    };

    _mockIntegrationDataProvider
        .GetScenariosAsync(Arg.Any<CancellationToken>())
        .Returns(expectedScenarios);

    // Act
    var result = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    var scenarios = result.ToList();
    Assert.Equal(3, scenarios.Count);
  }

  [Fact]
  public async Task ListAsync_UsesCaching_OnSecondCall()
  {
    // Arrange
    var expectedScenarios = new List<Contracts.DTO.ScenarioDTO>
    {
      new Contracts.DTO.ScenarioDTO { Id = Guid.NewGuid(), Code = "AVA-CACHE-TEST" }
    };

    _mockIntegrationDataProvider
        .GetScenariosAsync(Arg.Any<CancellationToken>())
        .Returns(expectedScenarios);

    // Act
    var result1 = await _sut.ListAsync(Datasource.AVAPlace);
    var result2 = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    Assert.Equal(result1, result2);

    await _mockIntegrationDataProvider
        .Received(1)
        .GetScenariosAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ListAsync_UsesDifferentCacheKeys_ForDifferentDatasources()
  {
    // Arrange
    var avaPlaceScenarios = new List<Contracts.DTO.ScenarioDTO>
    {
      new Contracts.DTO.ScenarioDTO { Id = Guid.NewGuid(), Code = "AVA-001" }
    };

    _mockIntegrationDataProvider
        .GetScenariosAsync(Arg.Any<CancellationToken>())
        .Returns(avaPlaceScenarios);

    var dbScenario = new Scenario(Guid.NewGuid());
    var codeProperty = typeof(Scenario).GetProperty("Code");

    // ✅ ZMĚNA: Použít unikátní kód pro každý běh testu
    var uniqueCode = $"DB-TEST-{Guid.NewGuid():N}";
    codeProperty!.SetValue(dbScenario, uniqueCode);

    _dbContext.Scenarios.Add(dbScenario);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var avaPlaceResult = await _sut.ListAsync(Datasource.AVAPlace);
    var databaseResult = await _sut.ListAsync(Datasource.Database);

    // Assert
    Assert.Single(avaPlaceResult);
    Assert.Single(databaseResult);
    Assert.NotEqual(avaPlaceResult.First().Code, databaseResult.First().Code);
    Assert.Equal("AVA-001", avaPlaceResult.First().Code);
    Assert.Equal(uniqueCode, databaseResult.First().Code);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_CachingWorks()
  {
    // Arrange
    var scenario = new Scenario(Guid.NewGuid());
    var codeProperty = typeof(Scenario).GetProperty("Code");

    // ✅ Use unique code here too
    var uniqueCode = $"CACHE-TEST-{Guid.NewGuid():N}";
    codeProperty!.SetValue(scenario, uniqueCode);

    _dbContext.Scenarios.Add(scenario);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result1 = await _sut.ListAsync(Datasource.Database);

    var newScenario = new Scenario(Guid.NewGuid());
    var newCode = $"NEW-SCENARIO-{Guid.NewGuid():N}";
    codeProperty.SetValue(newScenario, newCode);
    _dbContext.Scenarios.Add(newScenario);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    var result2 = await _sut.ListAsync(Datasource.Database);

    // Assert
    Assert.Single(result1);
    Assert.Single(result2);
    Assert.Equal(uniqueCode, result2.First().Code);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_LoadsOnlyNeededFeatures()
  {
    // Arrange
    var feature1 = new Feature(Guid.NewGuid(), "FEATURE-USED-1");
    var feature2 = new Feature(Guid.NewGuid(), "FEATURE-USED-2");
    var feature3 = new Feature(Guid.NewGuid(), "FEATURE-NOT-USED");

    _dbContext.Features.AddRange(feature1, feature2, feature3);

    var scenario = new Scenario(Guid.NewGuid());
    var codeProperty = typeof(Scenario).GetProperty("Code");
    codeProperty!.SetValue(scenario, "SCEN-SELECTIVE");
    scenario.SetInputFeature(feature1.Id);
    scenario.SetOutputFeature(feature2.Id);

    _dbContext.Scenarios.Add(scenario);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    var scenarios = result.ToList();
    Assert.Single(scenarios);

    Assert.NotNull(scenarios[0].InputFeatureSummary);
    Assert.Equal("FEATURE-USED-1", scenarios[0].InputFeatureSummary!.Code);

    Assert.NotNull(scenarios[0].OutputFeatureSummary);
    Assert.Equal("FEATURE-USED-2", scenarios[0].OutputFeatureSummary!.Code);
  }
}
