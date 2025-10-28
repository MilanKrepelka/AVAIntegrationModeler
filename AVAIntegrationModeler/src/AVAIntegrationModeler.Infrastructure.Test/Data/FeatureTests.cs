using AVAIntegrationModeler.Domain.FeatureAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Integrační testy pro databázovou vrstvu Feature aggregate.
/// </summary>
[Collection("FeatureTestCollection")]
public class FeatureTests : BaseDbTests
{
  private readonly EfRepository<Feature> _repository;

  public FeatureTests(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture)
    : base(testOutputHelper, fixture)
  {
    _repository = GetRepository<Feature>();
  }

  [Fact]
  public async Task AddFeature_ShouldPersistToDatabase()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var featureCode = "CUSTOMER_SYNC";
    var feature = new Feature(featureId, featureCode);
    var name = new LocalizedValue { CzechValue = "Synchronizace zákazníků", EnglishValue = "Customer Synchronization" };
    var description = new LocalizedValue { CzechValue = "Synchronizace dat zákazníků", EnglishValue = "Customer data synchronization" };
    
    feature.SetName(name)
           .SetDescription(description);

    // Act
    await _repository.AddAsync(feature, CancellationToken.None);

    // Assert
    var savedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    
    savedFeature.ShouldNotBeNull();
    savedFeature.Id.ShouldBe(featureId);
    savedFeature.Code.ShouldBe(featureCode);
    savedFeature.Name.CzechValue.ShouldBe("Synchronizace zákazníků");
    savedFeature.Name.EnglishValue.ShouldBe("Customer Synchronization");
  }

  [Fact]
  public async Task AddFeature_WithMinimalData_ShouldSucceed()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var featureCode = "MINIMAL_FEATURE";
    var feature = new Feature(featureId, featureCode);

    // Act
    await _repository.AddAsync(feature, CancellationToken.None);

    // Assert
    var savedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    
    savedFeature.ShouldNotBeNull();
    savedFeature.Id.ShouldBe(featureId);
    savedFeature.Code.ShouldBe(featureCode);
  }

  [Fact]
  public async Task UpdateFeature_ShouldPersistChanges()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "ORIG_CODE");
    var originalName = new LocalizedValue { CzechValue = "Původní název", EnglishValue = "Original Name" };
    feature.SetName(originalName)
           .SetDescription(new LocalizedValue { CzechValue = "Původní popis", EnglishValue = "Original Description" });
    
    await _repository.AddAsync(feature, CancellationToken.None);

    // Detach to simulate fetching from database
    DbContext.Entry(feature).State = EntityState.Detached;

    // Act
    var fetchedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    fetchedFeature.ShouldNotBeNull();
    
    var newName = new LocalizedValue { CzechValue = "Nový název", EnglishValue = "New Name" };
    var newDescription = new LocalizedValue { CzechValue = "Nový popis", EnglishValue = "New Description" };
    
    fetchedFeature.SetCode("NEW_CODE")
                  .SetName(newName)
                  .SetDescription(newDescription);

    await _repository.UpdateAsync(fetchedFeature, CancellationToken.None);

    // Assert
    var updatedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    
    updatedFeature.ShouldNotBeNull();
    updatedFeature.Code.ShouldBe("NEW_CODE");
    updatedFeature.Name.CzechValue.ShouldBe("Nový název");
    updatedFeature.Name.EnglishValue.ShouldBe("New Name");
  }

  [Fact]
  public async Task DeleteFeature_ShouldRemoveFromDatabase()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "DELETE_TEST");
    var name = new LocalizedValue { CzechValue = "Feature ke smazání", EnglishValue = "Feature to Delete" };
    feature.SetName(name);
    
    await _repository.AddAsync(feature, CancellationToken.None);

    // Verify it was added
    var addedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    addedFeature.ShouldNotBeNull();

    // Act
    await _repository.DeleteAsync(addedFeature, CancellationToken.None);

    // Assert
    var deletedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    deletedFeature.ShouldBeNull();
  }

  [Fact]
  public async Task AddFeature_WithIncludedFeatures_ShouldPersistRelations()
  {
    // Arrange - vytvoříme referenční features
    var referencedFeature1Id = Guid.NewGuid();
    var referencedFeature1 = new Feature(referencedFeature1Id, "AUTH_FEATURE");
    referencedFeature1.SetName(new LocalizedValue { CzechValue = "Autentizace", EnglishValue = "Authentication" });
    await _repository.AddAsync(referencedFeature1, CancellationToken.None);

    var referencedFeature2Id = Guid.NewGuid();
    var referencedFeature2 = new Feature(referencedFeature2Id, "LOGGING_FEATURE");
    referencedFeature2.SetName(new LocalizedValue { CzechValue = "Logování", EnglishValue = "Logging" });
    await _repository.AddAsync(referencedFeature2, CancellationToken.None);

    // Vytvoříme hlavní feature se zahrnutými features
    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "MAIN_FEATURE");
    feature.SetName(new LocalizedValue { CzechValue = "Hlavní feature", EnglishValue = "Main Feature" });
    feature.AddIncludedFeature(referencedFeature1Id, consumeOnly: false)
           .AddIncludedFeature(referencedFeature2Id, consumeOnly: true);

    // Act
    await _repository.AddAsync(feature, CancellationToken.None);

    // Assert
    var savedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    
    savedFeature.ShouldNotBeNull();
    savedFeature.IncludedFeatures.Count.ShouldBe(2);
    savedFeature.IncludedFeatures.ShouldContain(f => f.FeatureId == referencedFeature1Id);
    savedFeature.IncludedFeatures.ShouldContain(f => f.FeatureId == referencedFeature2Id);

    var authFeature = savedFeature.GetIncludedFeature(referencedFeature1Id);
    authFeature.ShouldNotBeNull();
    authFeature.ConsumeOnly.ShouldBeFalse();

    var loggingFeature = savedFeature.GetIncludedFeature(referencedFeature2Id);
    loggingFeature.ShouldNotBeNull();
    loggingFeature.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public async Task AddFeature_WithIncludedModels_ShouldPersistRelations()
  {
    // Arrange - vytvoříme referenční modely
    var model1Id = Guid.NewGuid();
    var model2Id = Guid.NewGuid();

    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "CUSTOMER_FEATURE");
    feature.SetName(new LocalizedValue { CzechValue = "Zákaznická feature", EnglishValue = "Customer Feature" });
    feature.AddIncludedModel(model1Id, consumeOnly: false)
           .AddIncludedModel(model2Id, consumeOnly: true);

    // Act
    await _repository.AddAsync(feature, CancellationToken.None);

    // Assert
    var savedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    
    savedFeature.ShouldNotBeNull();
    savedFeature.IncludedModels.Count.ShouldBe(2);
    savedFeature.IncludedModels.ShouldContain(m => m.ModelId == model1Id);
    savedFeature.IncludedModels.ShouldContain(m => m.ModelId == model2Id);

    var model1 = savedFeature.GetIncludedModel(model1Id);
    model1.ShouldNotBeNull();
    model1.ConsumeOnly.ShouldBeFalse();

    var model2 = savedFeature.GetIncludedModel(model2Id);
    model2.ShouldNotBeNull();
    model2.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public async Task RemoveIncludedFeature_ShouldPersistRemoval()
  {
    // Arrange
    var referencedFeature1Id = Guid.NewGuid();
    var referencedFeature1 = new Feature(referencedFeature1Id, "FEATURE_1");
    referencedFeature1.SetName(new LocalizedValue { CzechValue = "Feature 1", EnglishValue = "Feature 1" });
    await _repository.AddAsync(referencedFeature1, CancellationToken.None);

    var referencedFeature2Id = Guid.NewGuid();
    var referencedFeature2 = new Feature(referencedFeature2Id, "FEATURE_2");
    referencedFeature2.SetName(new LocalizedValue { CzechValue = "Feature 2", EnglishValue = "Feature 2" });
    await _repository.AddAsync(referencedFeature2, CancellationToken.None);

    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "PARENT_FEATURE");
    feature.SetName(new LocalizedValue { CzechValue = "Rodičovská feature", EnglishValue = "Parent Feature" });
    feature.AddIncludedFeature(referencedFeature1Id)
           .AddIncludedFeature(referencedFeature2Id);

    await _repository.AddAsync(feature, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Detach
    DbContext.ChangeTracker.Clear();

    // Act - odebereme jednu zahrnutou feature
    var fetchedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    fetchedFeature.ShouldNotBeNull();
    
    var includedCountBefore = fetchedFeature.IncludedFeatures.Count;
    this._testOutputHelper.WriteLine($"Included features count before remove: {includedCountBefore}");
    includedCountBefore.ShouldBe(2);
    
    fetchedFeature.RemoveIncludedFeature(referencedFeature1Id);
    
    await _repository.UpdateAsync(fetchedFeature, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    var updatedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    
    updatedFeature.ShouldNotBeNull();
    this._testOutputHelper.WriteLine($"Included features count after remove: {updatedFeature.IncludedFeatures.Count}");
    
    updatedFeature.IncludedFeatures.Count.ShouldBe(1);
    updatedFeature.IncludedFeatures.ShouldNotContain(f => f.FeatureId == referencedFeature1Id);
    updatedFeature.IncludedFeatures.ShouldContain(f => f.FeatureId == referencedFeature2Id);
  }

  [Fact]
  public async Task RemoveIncludedModel_ShouldPersistRemoval()
  {
    // Arrange
    var model1Id = Guid.NewGuid();
    var model2Id = Guid.NewGuid();
    var model3Id = Guid.NewGuid();

    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "MODEL_FEATURE");
    feature.SetName(new LocalizedValue { CzechValue = "Modelová feature", EnglishValue = "Model Feature" });
    feature.AddIncludedModel(model1Id)
           .AddIncludedModel(model2Id)
           .AddIncludedModel(model3Id);

    await _repository.AddAsync(feature, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Detach
    DbContext.ChangeTracker.Clear();

    // Act - odebereme jeden model
    var fetchedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    fetchedFeature.ShouldNotBeNull();
    fetchedFeature.IncludedModels.Count.ShouldBe(3);
    
    fetchedFeature.RemoveIncludedModel(model2Id);
    
    await _repository.UpdateAsync(fetchedFeature, CancellationToken.None);
    await _repository.SaveChangesAsync(CancellationToken.None);

    // Assert
    var updatedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    
    updatedFeature.ShouldNotBeNull();
    updatedFeature.IncludedModels.Count.ShouldBe(2);
    updatedFeature.IncludedModels.ShouldNotContain(m => m.ModelId == model2Id);
    updatedFeature.IncludedModels.ShouldContain(m => m.ModelId == model1Id);
    updatedFeature.IncludedModels.ShouldContain(m => m.ModelId == model3Id);
  }

  [Fact]
  public void AddFeature_CannotIncludeItself_ShouldThrowException()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "SELF_TEST");

    // Act & Assert
    Should.Throw<InvalidOperationException>(() =>
      feature.AddIncludedFeature(featureId));
  }

  [Fact]
  public async Task AddFeature_DuplicateIncludedFeature_ShouldNotAddTwice()
  {
    // Arrange
    var referencedFeatureId = Guid.NewGuid();
    var referencedFeature = new Feature(referencedFeatureId, "REF_FEATURE");
    referencedFeature.SetName(new LocalizedValue { CzechValue = "Referenční feature", EnglishValue = "Reference Feature" });
    await _repository.AddAsync(referencedFeature, CancellationToken.None);

    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "MAIN_FEATURE");
    feature.SetName(new LocalizedValue { CzechValue = "Hlavní feature", EnglishValue = "Main Feature" });
    
    // Act - přidáme stejnou feature dvakrát
    feature.AddIncludedFeature(referencedFeatureId);
    feature.AddIncludedFeature(referencedFeatureId); // duplicitní

    await _repository.AddAsync(feature, CancellationToken.None);

    // Assert
    var savedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    
    savedFeature.ShouldNotBeNull();
    savedFeature.IncludedFeatures.Count.ShouldBe(1); // pouze jedna
  }

  [Fact]
  public void Feature_SetCode_EmptyString_ShouldThrowException()
  {
    // Arrange & Act & Assert
    Should.Throw<ArgumentException>(() =>
      new Feature(Guid.NewGuid(), string.Empty));
  }

  [Fact]
  public async Task Feature_FluentAPI_ShouldChainMethods()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var name = new LocalizedValue { CzechValue = "Fluent název", EnglishValue = "Fluent Name" };
    var description = new LocalizedValue { CzechValue = "Fluent popis", EnglishValue = "Fluent Description" };

    // Act
    var feature = new Feature(featureId, "FLUENT_TEST")
      .SetName(name)
      .SetDescription(description);

    await _repository.AddAsync(feature, CancellationToken.None);

    // Assert
    var savedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    
    savedFeature.ShouldNotBeNull();
    savedFeature.Code.ShouldBe("FLUENT_TEST");
    savedFeature.Name.CzechValue.ShouldBe("Fluent název");
    savedFeature.Description.EnglishValue.ShouldBe("Fluent Description");
  }

  [Fact]
  public async Task ListFeatures_ShouldReturnAll()
  {
    // Arrange
    var f1 = new Feature(Guid.NewGuid(), "FEATURE_1")
      .SetName(new LocalizedValue { CzechValue = "Feature 1", EnglishValue = "Feature 1" });
    var f2 = new Feature(Guid.NewGuid(), "FEATURE_2")
      .SetName(new LocalizedValue { CzechValue = "Feature 2", EnglishValue = "Feature 2" });
    var f3 = new Feature(Guid.NewGuid(), "FEATURE_3")
      .SetName(new LocalizedValue { CzechValue = "Feature 3", EnglishValue = "Feature 3" });

    await _repository.AddAsync(f1, CancellationToken.None);
    await _repository.AddAsync(f2, CancellationToken.None);
    await _repository.AddAsync(f3, CancellationToken.None);

    // Act
    var allFeatures = await _repository.ListAsync(CancellationToken.None);

    // Assert
    allFeatures.Count.ShouldBe(3);
    allFeatures.ShouldContain(f => f.Code == "FEATURE_1");
    allFeatures.ShouldContain(f => f.Code == "FEATURE_2");
    allFeatures.ShouldContain(f => f.Code == "FEATURE_3");
  }

  [Fact]
  public async Task DeleteFeature_WithIncludedItems_ShouldCascadeDelete()
  {
    // Arrange
    var referencedFeatureId = Guid.NewGuid();
    var referencedFeature = new Feature(referencedFeatureId, "REF_FEATURE");
    referencedFeature.SetName(new LocalizedValue { CzechValue = "Referenční", EnglishValue = "Reference" });
    await _repository.AddAsync(referencedFeature, CancellationToken.None);

    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "CASCADE_TEST");
    feature.SetName(new LocalizedValue { CzechValue = "Test kaskády", EnglishValue = "Cascade Test" });
    feature.AddIncludedFeature(referencedFeatureId);
    feature.AddIncludedModel(Guid.NewGuid());

    var includedFeaturesCountBefore = feature.IncludedFeatures.Count;
    var includedModelsCountBefore = feature.IncludedModels.Count;

    await _repository.AddAsync(feature, CancellationToken.None);

    // Act
    await _repository.DeleteAsync(feature, CancellationToken.None);

    // Assert
    var deletedFeature = await _repository.GetByIdAsync(featureId, CancellationToken.None);
    deletedFeature.ShouldBeNull();
    
    // Ověření, že referenční feature stále existuje (nesmazala se)
    var referencedFeatureAfterDelete = await _repository.GetByIdAsync(referencedFeatureId, CancellationToken.None);
    referencedFeatureAfterDelete.ShouldNotBeNull();
    
    // Test potvrzuje, že feature byla smazána včetně všech owned entities
    includedFeaturesCountBefore.ShouldBeGreaterThan(0);
    includedModelsCountBefore.ShouldBeGreaterThan(0);
  }
}
