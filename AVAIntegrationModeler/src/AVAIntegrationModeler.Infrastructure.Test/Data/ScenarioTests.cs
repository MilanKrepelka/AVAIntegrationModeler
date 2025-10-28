using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Integrační testy pro Scenario aggregate v databázi.
/// </summary>
[Collection("ScenarioTestCollection")]
public class ScenarioTests : BaseDbTests
{
  private readonly EfRepository<Scenario> _repository;

  public ScenarioTests(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture)
    : base(testOutputHelper, fixture)
  {
    _repository = GetRepository<Scenario>();
  }

  #region Add Tests

  [Fact]
  public async Task AddAsync_SingleScenario_ShouldPersistCorrectly()
  {
    // Arrange
    var scenarioId = Guid.NewGuid();
    var scenario = new Scenario(scenarioId)
      .SetCode("TEST_SCENARIO")
      .SetName(new LocalizedValue
      {
        CzechValue = "Testovací scénář",
        EnglishValue = "Test Scenario"
      })
      .SetDescription(new LocalizedValue
      {
        CzechValue = "Popis testovacího scénáře",
        EnglishValue = "Test scenario description"
      });

    // Act
    await _repository.AddAsync(scenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var savedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);

    savedScenario.ShouldNotBeNull();
    savedScenario.Id.ShouldBe(scenarioId);
    savedScenario.Code.ShouldBe("TEST_SCENARIO");
    savedScenario.Name.CzechValue.ShouldBe("Testovací scénář");
    savedScenario.Name.EnglishValue.ShouldBe("Test Scenario");
    savedScenario.Description.CzechValue.ShouldBe("Popis testovacího scénáře");
    savedScenario.Description.EnglishValue.ShouldBe("Test scenario description");
  }

  [Fact]
  public async Task AddAsync_ScenarioWithInputAndOutputFeatures_ShouldPersistCorrectly()
  {
    // Arrange
    var inputFeatureId = Guid.NewGuid();
    var outputFeatureId = Guid.NewGuid();
    var scenarioId = Guid.NewGuid();

    var scenario = new Scenario(scenarioId)
      .SetCode("INTEGRATION_FLOW")
      .SetName(new LocalizedValue { CzechValue = "Integrační tok", EnglishValue = "Integration Flow" })
      .SetInputFeature(inputFeatureId)
      .SetOutputFeature(outputFeatureId);

    // Act
    await _repository.AddAsync(scenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var savedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);

    savedScenario.ShouldNotBeNull();
    savedScenario.InputFeature.ShouldBe(inputFeatureId);
    savedScenario.OutputFeature.ShouldBe(outputFeatureId);
  }

  [Fact]
  public async Task AddAsync_ScenarioWithNullFeatures_ShouldPersistCorrectly()
  {
    // Arrange
    var scenarioId = Guid.NewGuid();
    var scenario = new Scenario(scenarioId)
      .SetCode("NULL_FEATURES")
      .SetName(new LocalizedValue { CzechValue = "Bez features", EnglishValue = "No Features" })
      .SetInputFeature(null)
      .SetOutputFeature(null);

    // Act
    await _repository.AddAsync(scenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var savedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);

    savedScenario.ShouldNotBeNull();
    savedScenario.InputFeature.ShouldBeNull();
    savedScenario.OutputFeature.ShouldBeNull();
  }

  #endregion

  #region Update Tests

  [Fact]
  public async Task UpdateAsync_ChangeCode_ShouldPersistChanges()
  {
    // Arrange
    var scenarioId = Guid.NewGuid();
    var scenario = new Scenario(scenarioId)
      .SetCode("ORIGINAL_CODE")
      .SetName(new LocalizedValue { CzechValue = "Původní", EnglishValue = "Original" });

    await _repository.AddAsync(scenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Act
    DbContext.ChangeTracker.Clear();
    var fetchedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);
    fetchedScenario.ShouldNotBeNull();

    fetchedScenario.SetCode("UPDATED_CODE");
    await _repository.UpdateAsync(fetchedScenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var updatedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);
    updatedScenario.ShouldNotBeNull();
    updatedScenario.Code.ShouldBe("UPDATED_CODE");
  }

  [Fact]
  public async Task UpdateAsync_ChangeLocalizedName_ShouldPersistChanges()
  {
    // Arrange
    var scenarioId = Guid.NewGuid();
    var scenario = new Scenario(scenarioId)
      .SetCode("TEST")
      .SetName(new LocalizedValue { CzechValue = "Původní název", EnglishValue = "Original Name" });

    await _repository.AddAsync(scenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Act
    DbContext.ChangeTracker.Clear();
    var fetchedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);
    fetchedScenario.ShouldNotBeNull();

    fetchedScenario.SetName(new LocalizedValue
    {
      CzechValue = "Nový název",
      EnglishValue = "New Name"
    });
    await _repository.UpdateAsync(fetchedScenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var updatedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);
    updatedScenario.ShouldNotBeNull();
    updatedScenario.Name.CzechValue.ShouldBe("Nový název");
    updatedScenario.Name.EnglishValue.ShouldBe("New Name");
  }

  [Fact]
  public async Task UpdateAsync_ChangeFeatures_ShouldPersistChanges()
  {
    // Arrange
    var scenarioId = Guid.NewGuid();
    var originalInputId = Guid.NewGuid();
    var scenario = new Scenario(scenarioId)
      .SetCode("FEATURE_TEST")
      .SetName(new LocalizedValue { CzechValue = "Test", EnglishValue = "Test" })
      .SetInputFeature(originalInputId)
      .SetOutputFeature(null);

    await _repository.AddAsync(scenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Act
    DbContext.ChangeTracker.Clear();
    var fetchedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);
    fetchedScenario.ShouldNotBeNull();

    var newOutputId = Guid.NewGuid();
    fetchedScenario
      .SetInputFeature(null)
      .SetOutputFeature(newOutputId);

    await _repository.UpdateAsync(fetchedScenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var updatedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);
    updatedScenario.ShouldNotBeNull();
    updatedScenario.InputFeature.ShouldBeNull();
    updatedScenario.OutputFeature.ShouldBe(newOutputId);
  }

  #endregion

  #region Delete Tests

  [Fact]
  public async Task DeleteAsync_ExistingScenario_ShouldRemoveFromDatabase()
  {
    // Arrange
    var scenarioId = Guid.NewGuid();
    var scenario = new Scenario(scenarioId)
      .SetCode("TO_DELETE")
      .SetName(new LocalizedValue { CzechValue = "Smazat", EnglishValue = "Delete" });

    await _repository.AddAsync(scenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Act
    DbContext.ChangeTracker.Clear();
    var fetchedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);
    fetchedScenario.ShouldNotBeNull();

    await _repository.DeleteAsync(fetchedScenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var deletedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);
    deletedScenario.ShouldBeNull();
  }

  #endregion

  #region List Tests

  [Fact]
  public async Task ListAsync_MultipleScenarios_ShouldReturnAll()
  {
    // Arrange
    var scenario1 = new Scenario(Guid.NewGuid())
      .SetCode("SCENARIO_1")
      .SetName(new LocalizedValue { CzechValue = "Scénář 1", EnglishValue = "Scenario 1" });

    var scenario2 = new Scenario(Guid.NewGuid())
      .SetCode("SCENARIO_2")
      .SetName(new LocalizedValue { CzechValue = "Scénář 2", EnglishValue = "Scenario 2" });

    var scenario3 = new Scenario(Guid.NewGuid())
      .SetCode("SCENARIO_3")
      .SetName(new LocalizedValue { CzechValue = "Scénář 3", EnglishValue = "Scenario 3" });

    await _repository.AddAsync(scenario1, CancellationToken.None);
    await _repository.AddAsync(scenario2, CancellationToken.None);
    await _repository.AddAsync(scenario3, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Act
    DbContext.ChangeTracker.Clear();
    var allScenarios = await _repository.ListAsync(CancellationToken.None);

    // Assert
    allScenarios.Count.ShouldBeGreaterThanOrEqualTo(3);
    allScenarios.ShouldContain(s => s.Code == "SCENARIO_1");
    allScenarios.ShouldContain(s => s.Code == "SCENARIO_2");
    allScenarios.ShouldContain(s => s.Code == "SCENARIO_3");
  }

  #endregion

  #region Fluent API Tests

  [Fact]
  public async Task FluentAPI_ChainedMethodCalls_ShouldWorkCorrectly()
  {
    // Arrange & Act
    var scenarioId = Guid.NewGuid();
    var inputFeatureId = Guid.NewGuid();
    var outputFeatureId = Guid.NewGuid();

    var scenario = new Scenario(scenarioId)
      .SetCode("FLUENT_TEST")
      .SetName(new LocalizedValue { CzechValue = "Fluent test", EnglishValue = "Fluent Test" })
      .SetDescription(new LocalizedValue { CzechValue = "Popis", EnglishValue = "Description" })
      .SetInputFeature(inputFeatureId)
      .SetOutputFeature(outputFeatureId);

    await _repository.AddAsync(scenario, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    DbContext.ChangeTracker.Clear();
    var savedScenario = await _repository.GetByIdAsync(scenarioId, CancellationToken.None);

    savedScenario.ShouldNotBeNull();
    savedScenario.Code.ShouldBe("FLUENT_TEST");
    savedScenario.Name.CzechValue.ShouldBe("Fluent test");
    savedScenario.Name.EnglishValue.ShouldBe("Fluent Test");
    savedScenario.Description.CzechValue.ShouldBe("Popis");
    savedScenario.Description.EnglishValue.ShouldBe("Description");
    savedScenario.InputFeature.ShouldBe(inputFeatureId);
    savedScenario.OutputFeature.ShouldBe(outputFeatureId);
  }

  #endregion

  #region Validation Tests

  [Fact]
  public async Task AddAsync_UniqueCodeConstraint_ShouldEnforceUniqueness()
  {
    // Arrange
    var scenario1 = new Scenario(Guid.NewGuid())
      .SetCode("DUPLICATE_CODE")
      .SetName(new LocalizedValue { CzechValue = "První", EnglishValue = "First" });

    var scenario2 = new Scenario(Guid.NewGuid())
      .SetCode("DUPLICATE_CODE")
      .SetName(new LocalizedValue { CzechValue = "Druhý", EnglishValue = "Second" });

    await _repository.AddAsync(scenario1, CancellationToken.None);
    

    
    await Should.ThrowAsync<DbUpdateException>(async () =>
    {
      // Act & Assert
      await _repository.AddAsync(scenario2, CancellationToken.None);
      
    });
  }

  #endregion
}
