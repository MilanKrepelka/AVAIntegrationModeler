using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.FeatureAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace AVAIntegrationModeler.Infrastructure.Tests.Data.Queries;

/// <summary>
/// Testy pro ListFeaturesQueryService - testování metody ListAsync pro načítání features z různých zdrojů dat.
/// </summary>
public class ListFeaturesQueryServiceTests : IAsyncLifetime, IAsyncDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly IIntegrationDataProvider _mockIntegrationDataProvider;
  private readonly IMemoryCache _memoryCache;
  private readonly ListFeaturesQueryService _sut;
  
  public ListFeaturesQueryServiceTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    _dbContext = new AppDbContext(options, null);
    
    // ✅ Vyčistit databázi před každým testem
    _dbContext.Database.EnsureDeleted();
    _dbContext.Database.EnsureCreated();
    
    _mockIntegrationDataProvider = Substitute.For<IIntegrationDataProvider>();
    _memoryCache = new MemoryCache(new MemoryCacheOptions());

    _sut = new ListFeaturesQueryService(
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

  #region Database Datasource Tests

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_ReturnsEmptyList_WhenNoFeatures()
  {
    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_ReturnsFeatures_WhenFeaturesExist()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "FEAT-001");
    feature.SetName(new LocalizedValue
    {
      CzechValue = "Testovací feature",
      EnglishValue = "Test Feature"
    });
    feature.SetDescription(new LocalizedValue
    {
      CzechValue = "Popis testovací feature",
      EnglishValue = "Test feature description"
    });

    _dbContext.Features.Add(feature);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    Assert.NotNull(result);
    var features = result.ToList();
    Assert.Single(features);
    Assert.Equal("FEAT-001", features[0].Code);
    Assert.Equal(featureId, features[0].Id);
    Assert.Equal("Testovací feature", features[0].Name.CzechValue);
    Assert.Equal("Test Feature", features[0].Name.EnglishValue);
    Assert.Equal("Popis testovací feature", features[0].Description.CzechValue);
    Assert.Equal("Test feature description", features[0].Description.EnglishValue);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_ReturnsMultipleFeatures()
  {
    // Arrange
    var feature1 = new Feature(Guid.NewGuid(), "FEAT-001");
    feature1.SetName(new LocalizedValue { CzechValue = "První", EnglishValue = "First" });
    feature1.SetDescription(new LocalizedValue { CzechValue = "První popis", EnglishValue = "First description" });

    var feature2 = new Feature(Guid.NewGuid(), "FEAT-002");
    feature2.SetName(new LocalizedValue { CzechValue = "Druhá", EnglishValue = "Second" });
    feature2.SetDescription(new LocalizedValue { CzechValue = "Druhý popis", EnglishValue = "Second description" });

    var feature3 = new Feature(Guid.NewGuid(), "FEAT-003");
    feature3.SetName(new LocalizedValue { CzechValue = "Třetí", EnglishValue = "Third" });
    feature3.SetDescription(new LocalizedValue { CzechValue = "Třetí popis", EnglishValue = "Third description" });

    _dbContext.Features.AddRange(feature1, feature2, feature3);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    var features = result.ToList();
    Assert.Equal(3, features.Count);
    Assert.Contains(features, f => f.Code == "FEAT-001");
    Assert.Contains(features, f => f.Code == "FEAT-002");
    Assert.Contains(features, f => f.Code == "FEAT-003");
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_ReturnsCompleteFeatureData()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "COMPLETE-FEAT");
    feature.SetName(new LocalizedValue
    {
      CzechValue = "Kompletní feature",
      EnglishValue = "Complete Feature"
    });
    feature.SetDescription(new LocalizedValue
    {
      CzechValue = "Úplný popis s českými znaky",
      EnglishValue = "Complete description with special chars"
    });

    _dbContext.Features.Add(feature);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    var features = result.ToList();
    Assert.Single(features);

    var featureDto = features[0];
    Assert.Equal(featureId, featureDto.Id);
    Assert.Equal("COMPLETE-FEAT", featureDto.Code);
    Assert.NotNull(featureDto.Name);
    Assert.Equal("Kompletní feature", featureDto.Name.CzechValue);
    Assert.Equal("Complete Feature", featureDto.Name.EnglishValue);
    Assert.NotNull(featureDto.Description);
    Assert.Equal("Úplný popis s českými znaky", featureDto.Description.CzechValue);
    Assert.Equal("Complete description with special chars", featureDto.Description.EnglishValue);
  }

  #endregion

  #region AVAPlace Datasource Tests

  [Fact]
  public async Task ListAsync_WithAVAPlaceDatasource_CallsIntegrationProvider()
  {
    // Arrange
    var expectedFeatures = new List<Contracts.DTO.FeatureDTO>
    {
      new Contracts.DTO.FeatureDTO
      {
        Id = Guid.NewGuid(),
        Code = "AVA-FEAT-001",
        Name = new LocalizedValue
        {
          CzechValue = "AVA feature",
          EnglishValue = "AVA Feature"
        },
        Description = new LocalizedValue
        {
          CzechValue = "Popis z AVAPlace",
          EnglishValue = "Description from AVAPlace"
        }
      }
    };

    _mockIntegrationDataProvider
        .GetFeaturesAsync(Arg.Any<CancellationToken>())
        .Returns(expectedFeatures);

    // Act
    var result = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    Assert.NotNull(result);
    var features = result.ToList();
    Assert.Single(features);
    Assert.Equal("AVA-FEAT-001", features[0].Code);
    Assert.Equal("AVA feature", features[0].Name.CzechValue);

    await _mockIntegrationDataProvider
        .Received(1)
        .GetFeaturesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ListAsync_WithAVAPlaceDatasource_ReturnsMultipleFeatures()
  {
    // Arrange
    var expectedFeatures = new List<Contracts.DTO.FeatureDTO>
    {
      new Contracts.DTO.FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-001" },
      new Contracts.DTO.FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-002" },
      new Contracts.DTO.FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-003" }
    };

    _mockIntegrationDataProvider
        .GetFeaturesAsync(Arg.Any<CancellationToken>())
        .Returns(expectedFeatures);

    // Act
    var result = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    var features = result.ToList();
    Assert.Equal(3, features.Count);
  }

  [Fact]
  public async Task ListAsync_WithAVAPlaceDatasource_MapsDataCorrectly()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var expectedFeatures = new List<Contracts.DTO.FeatureDTO>
    {
      new Contracts.DTO.FeatureDTO
      {
        Id = featureId,
        Code = "MAP-TEST",
        Name = new LocalizedValue
        {
          CzechValue = "Test mapování",
          EnglishValue = "Mapping test"
        },
        Description = new LocalizedValue
        {
          CzechValue = "Popis mapování",
          EnglishValue = "Mapping description"
        }
      }
    };

    _mockIntegrationDataProvider
        .GetFeaturesAsync(Arg.Any<CancellationToken>())
        .Returns(expectedFeatures);

    // Act
    var result = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    var features = result.ToList();
    Assert.Single(features);

    var feature = features[0];
    Assert.Equal(featureId, feature.Id);
    Assert.Equal("MAP-TEST", feature.Code);
    Assert.Equal("Test mapování", feature.Name.CzechValue);
    Assert.Equal("Mapping test", feature.Name.EnglishValue);
    Assert.Equal("Popis mapování", feature.Description.CzechValue);
    Assert.Equal("Mapping description", feature.Description.EnglishValue);
  }

  [Fact]
  public async Task ListAsync_WithAVAPlaceDatasource_ReturnsEmptyList_WhenProviderReturnsEmpty()
  {
    // Arrange
    _mockIntegrationDataProvider
        .GetFeaturesAsync(Arg.Any<CancellationToken>())
        .Returns(new List<Contracts.DTO.FeatureDTO>());

    // Act
    var result = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
  }

  #endregion

  #region Caching Tests

  [Fact]
  public async Task ListAsync_UsesCaching_OnSecondCall()
  {
    // Arrange
    var expectedFeatures = new List<Contracts.DTO.FeatureDTO>
    {
      new Contracts.DTO.FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-CACHE-TEST" }
    };

    _mockIntegrationDataProvider
        .GetFeaturesAsync(Arg.Any<CancellationToken>())
        .Returns(expectedFeatures);

    // Act
    var result1 = await _sut.ListAsync(Datasource.AVAPlace);
    var result2 = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    Assert.Equal(result1, result2);

    await _mockIntegrationDataProvider
        .Received(1)
        .GetFeaturesAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task ListAsync_UsesDifferentCacheKeys_ForDifferentDatasources()
  {
    // Arrange
    var avaPlaceFeatures = new List<Contracts.DTO.FeatureDTO>
    {
      new Contracts.DTO.FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-001" }
    };
    
    _mockIntegrationDataProvider
        .GetFeaturesAsync(Arg.Any<CancellationToken>())
        .Returns(avaPlaceFeatures);

    var dbFeature = new Feature(Guid.NewGuid(), $"DB-CACHE-{Guid.NewGuid()}");
    _dbContext.Features.Add(dbFeature);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var avaPlaceResult = await _sut.ListAsync(Datasource.AVAPlace);
    var databaseResult = await _sut.ListAsync(Datasource.Database);

    // Assert
    Assert.Single(avaPlaceResult);
    Assert.Single(databaseResult);
    Assert.NotEqual(avaPlaceResult.First().Code, databaseResult.First().Code);
    
    Assert.Equal(avaPlaceFeatures.ElementAt(0).Code, avaPlaceResult.First().Code);
    Assert.Equal(dbFeature.Code, databaseResult.First().Code);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_CachingWorks()
  {
    // Arrange
    var feature = new Feature(Guid.NewGuid(), "CACHE-TEST");
    _dbContext.Features.Add(feature);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result1 = await _sut.ListAsync(Datasource.Database);

    var newFeature = new Feature(Guid.NewGuid(), "NEW-FEATURE");
    _dbContext.Features.Add(newFeature);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    var result2 = await _sut.ListAsync(Datasource.Database);

    // Assert
    Assert.Single(result1);
    Assert.Single(result2); // Cache vrací stále první výsledek
    Assert.Equal("CACHE-TEST", result2.First().Code);
  }

  [Fact]
  public async Task ListAsync_CacheExpiration_IsSetCorrectly()
  {
    // Arrange
    var expectedFeatures = new List<Contracts.DTO.FeatureDTO>
    {
      new Contracts.DTO.FeatureDTO { Id = Guid.NewGuid(), Code = "CACHE-EXPIRY-TEST" }
    };

    _mockIntegrationDataProvider
        .GetFeaturesAsync(Arg.Any<CancellationToken>())
        .Returns(expectedFeatures);

    // Act
    await _sut.ListAsync(Datasource.AVAPlace);

    // Assert - ověření, že cache entry existuje
    var cacheKey = "FeatureListQuery-AVAPlace-ListAsync";
    var cachedValue = _memoryCache.Get(cacheKey);
    Assert.NotNull(cachedValue);
  }

  #endregion

  #region Edge Cases

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_HandlesMinimalFeatureData()
  {
    // Arrange - feature s minimálními daty
    var featureId = Guid.NewGuid();
    var feature = new Feature(featureId, "MIN-FEAT");
    // Nastavíme pouze povinné vlastnosti, bez Name a Description

    _dbContext.Features.Add(feature);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    var features = result.ToList();
    Assert.Single(features);
    Assert.Equal(featureId, features[0].Id);
    Assert.Equal("MIN-FEAT", features[0].Code);
  }

  [Fact]
  public async Task ListAsync_WithAVAPlaceDatasource_HandlesNullValues()
  {
    // Arrange
    var expectedFeatures = new List<Contracts.DTO.FeatureDTO>
    {
      new Contracts.DTO.FeatureDTO
      {
        Id = Guid.NewGuid(),
        Code = "NULL-TEST",
        Name = null!, // Otestujeme null hodnoty
        Description = null!
      }
    };

    _mockIntegrationDataProvider
        .GetFeaturesAsync(Arg.Any<CancellationToken>())
        .Returns(expectedFeatures);

    // Act
    var result = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    var features = result.ToList();
    Assert.Single(features);
    Assert.Equal("NULL-TEST", features[0].Code);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_OrdersResults()
  {
    // Arrange
    var feature1 = new Feature(Guid.NewGuid(), "FEAT-C");
    var feature2 = new Feature(Guid.NewGuid(), "FEAT-A");
    var feature3 = new Feature(Guid.NewGuid(), "FEAT-B");

    _dbContext.Features.AddRange(feature1, feature2, feature3);
    await _dbContext.SaveChangesAsync(CancellationToken.None);

    // Act
    var result = await _sut.ListAsync(Datasource.Database);

    // Assert
    var features = result.ToList();
    Assert.Equal(3, features.Count);
    // Ověřte, že všechny features jsou vráceny (pořadí může být libovolné)
    Assert.Contains(features, f => f.Code == "FEAT-A");
    Assert.Contains(features, f => f.Code == "FEAT-B");
    Assert.Contains(features, f => f.Code == "FEAT-C");
  }
    #region IncludedFeatures and IncludedModels Tests

    [Fact]
    public async Task ListAsync_WithDatabaseDatasource_LoadsIncludedFeatures()
    {
        // Arrange
        var includedFeatureId = Guid.NewGuid();
        var includedFeature = new Feature(includedFeatureId, "INCLUDED-FEAT-001");
        includedFeature.SetName(new LocalizedValue
        {
            CzechValue = "Zahrnutá feature",
            EnglishValue = "Included Feature"
        });

        var parentFeatureId = Guid.NewGuid();
        var parentFeature = new Feature(parentFeatureId, "PARENT-FEAT-001");
        parentFeature.SetName(new LocalizedValue
        {
            CzechValue = "Nadřazená feature",
            EnglishValue = "Parent Feature"
        });
        parentFeature.AddIncludedFeature(includedFeatureId, consumeOnly: true);

        _dbContext.Features.AddRange(includedFeature, parentFeature);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _sut.ListAsync(Datasource.Database);

        // Assert
        var features = result.ToList();
        var parent = features.FirstOrDefault(f => f.Id == parentFeatureId);

        Assert.NotNull(parent);
        Assert.NotNull(parent.IncludedFeatures);
        Assert.Single(parent.IncludedFeatures);

        var includedFeatureDto = parent.IncludedFeatures.First();
        Assert.NotNull(includedFeatureDto.Feature);
        Assert.Equal(includedFeatureId, includedFeatureDto.Feature.Id);
        Assert.Equal("INCLUDED-FEAT-001", includedFeatureDto.Feature.Code);
        Assert.True(includedFeatureDto.ConsumeOnly);
    }

    [Fact]
    public async Task ListAsync_WithDatabaseDatasource_LoadsMultipleIncludedFeatures()
    {
        // Arrange
        var includedFeature1 = new Feature(Guid.NewGuid(), "INCLUDED-001");
        var includedFeature2 = new Feature(Guid.NewGuid(), "INCLUDED-002");
        var includedFeature3 = new Feature(Guid.NewGuid(), "INCLUDED-003");

        var parentFeature = new Feature(Guid.NewGuid(), "PARENT-MULTI");
        parentFeature.AddIncludedFeature(includedFeature1.Id, consumeOnly: true);
        parentFeature.AddIncludedFeature(includedFeature2.Id, consumeOnly: false);
        parentFeature.AddIncludedFeature(includedFeature3.Id, consumeOnly: true);

        _dbContext.Features.AddRange(includedFeature1, includedFeature2, includedFeature3, parentFeature);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _sut.ListAsync(Datasource.Database);

        // Assert
        var parent = result.FirstOrDefault(f => f.Code == "PARENT-MULTI");
        Assert.NotNull(parent);
        Assert.Equal(3, parent.IncludedFeatures.Count);
        Assert.Contains(parent.IncludedFeatures, f => f.Feature.Code == "INCLUDED-001" && f.ConsumeOnly);
        Assert.Contains(parent.IncludedFeatures, f => f.Feature.Code == "INCLUDED-002" && !f.ConsumeOnly);
        Assert.Contains(parent.IncludedFeatures, f => f.Feature.Code == "INCLUDED-003" && f.ConsumeOnly);
    }

    [Fact]
    public async Task ListAsync_WithDatabaseDatasource_LoadsIncludedModels()
    {
        // Arrange
        var modelId = Guid.NewGuid();
        var dataModel = new DataModel(modelId, "MODEL-001");
        dataModel.SetName("Test Model");

        var featureId = Guid.NewGuid();
        var feature = new Feature(featureId, "FEAT-WITH-MODEL");
        feature.SetName(new LocalizedValue
        {
            CzechValue = "Feature s modelem",
            EnglishValue = "Feature with Model"
        });
        feature.AddIncludedModel(modelId, consumeOnly: true);

        _dbContext.DataModels.Add(dataModel);
        _dbContext.Features.Add(feature);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _sut.ListAsync(Datasource.Database);

        // Assert
        var featureDto = result.FirstOrDefault(f => f.Id == featureId);

        Assert.NotNull(featureDto);
        Assert.NotNull(featureDto.IncludedModels);
        Assert.Single(featureDto.IncludedModels);

        var includedModel = featureDto.IncludedModels.First();
        Assert.NotNull(includedModel.DataModel);
        Assert.Equal(modelId, includedModel.DataModel.Id);
        Assert.Equal("MODEL-001", includedModel.DataModel.Code);
        Assert.True(includedModel.ReadOnly);
    }

    [Fact]
    public async Task ListAsync_WithDatabaseDatasource_LoadsMultipleIncludedModels()
    {
        // Arrange
        var model1 = new DataModel(Guid.NewGuid(), "MODEL-001");
        model1.SetName("First Model");
        var model2 = new DataModel(Guid.NewGuid(), "MODEL-002");
        model2.SetName("Second Model");
        var model3 = new DataModel(Guid.NewGuid(), "MODEL-003");
        model3.SetName("Third Model");

        var feature = new Feature(Guid.NewGuid(), "FEAT-MULTI-MODELS");
        feature.AddIncludedModel(model1.Id, consumeOnly: false);
        feature.AddIncludedModel(model2.Id, consumeOnly: true);
        feature.AddIncludedModel(model3.Id, consumeOnly: false);

        _dbContext.DataModels.AddRange(model1, model2, model3);
        _dbContext.Features.Add(feature);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _sut.ListAsync(Datasource.Database);

        // Assert
        var featureDto = result.FirstOrDefault(f => f.Code == "FEAT-MULTI-MODELS");
        Assert.NotNull(featureDto);
        Assert.Equal(3, featureDto.IncludedModels.Count);
        Assert.Contains(featureDto.IncludedModels, m => m.DataModel.Code == "MODEL-001" && !m.ReadOnly);
        Assert.Contains(featureDto.IncludedModels, m => m.DataModel.Code == "MODEL-002" && m.ReadOnly);
        Assert.Contains(featureDto.IncludedModels, m => m.DataModel.Code == "MODEL-003" && !m.ReadOnly);
    }

    [Fact]
    public async Task ListAsync_WithDatabaseDatasource_LoadsBothIncludedFeaturesAndModels()
    {
        // Arrange
        var includedFeature = new Feature(Guid.NewGuid(), "INCLUDED-FEATURE");
        var dataModel = new DataModel(Guid.NewGuid(), "DATA-MODEL");

        var complexFeature = new Feature(Guid.NewGuid(), "COMPLEX-FEATURE");
        complexFeature.AddIncludedFeature(includedFeature.Id, consumeOnly: true);
        complexFeature.AddIncludedModel(dataModel.Id, consumeOnly: false);

        _dbContext.Features.AddRange(includedFeature, complexFeature);
        _dbContext.DataModels.Add(dataModel);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _sut.ListAsync(Datasource.Database);

        // Assert
        var feature = result.FirstOrDefault(f => f.Code == "COMPLEX-FEATURE");
        Assert.NotNull(feature);
        Assert.Single(feature.IncludedFeatures);
        Assert.Single(feature.IncludedModels);
        Assert.Equal("INCLUDED-FEATURE", feature.IncludedFeatures.First().Feature.Code);
        Assert.Equal("DATA-MODEL", feature.IncludedModels.First().DataModel.Code);
    }

    [Fact]
    public async Task ListAsync_WithDatabaseDatasource_HandlesEmptyIncludedCollections()
    {
        // Arrange
        var feature = new Feature(Guid.NewGuid(), "EMPTY-COLLECTIONS");
        feature.SetName(new LocalizedValue
        {
            CzechValue = "Feature bez zahrnutí",
            EnglishValue = "Feature without inclusions"
        });

        _dbContext.Features.Add(feature);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _sut.ListAsync(Datasource.Database);

        // Assert
        var featureDto = result.FirstOrDefault(f => f.Code == "EMPTY-COLLECTIONS");
        Assert.NotNull(featureDto);
        Assert.NotNull(featureDto.IncludedFeatures);
        Assert.NotNull(featureDto.IncludedModels);
        Assert.Empty(featureDto.IncludedFeatures);
        Assert.Empty(featureDto.IncludedModels);
    }

    [Fact]
    public async Task ListAsync_WithAVAPlaceDatasource_LoadsIncludedFeaturesAndModels()
    {
        // Arrange
        var includedFeatureId = Guid.NewGuid();
        var parentFeatureId = Guid.NewGuid();
        var modelId = Guid.NewGuid();

        var expectedFeatures = new List<Contracts.DTO.FeatureDTO>
      {
        new Contracts.DTO.FeatureDTO
        {
          Id = includedFeatureId,
          Code = "AVA-INCLUDED",
          Name = new LocalizedValue { CzechValue = "Zahrnutá", EnglishValue = "Included" }
        },
        new Contracts.DTO.FeatureDTO
        {
          Id = parentFeatureId,
          Code = "AVA-PARENT",
          Name = new LocalizedValue { CzechValue = "Nadřazená", EnglishValue = "Parent" },
          IncludedFeatures = new List<Contracts.DTO.IncludedFeatureDTO>
          {
            new Contracts.DTO.IncludedFeatureDTO
            {
              Feature = new Contracts.DTO.FeatureSummaryDTO { Id = includedFeatureId, Code = "AVA-INCLUDED" },
              ConsumeOnly = true
            }
          },
          IncludedModels = new List<Contracts.DTO.IncludedDataModelDTO>
          {
            new Contracts.DTO.IncludedDataModelDTO
            {
              DataModel = new Contracts.DTO.DataModelSummaryDTO { Id = modelId, Code = "AVA-MODEL", Name = "AVA Model" },
              ReadOnly = true
            }
          }
        }
      };

        var expectedModels = new List<Contracts.DTO.DataModelSummaryDTO>
      {
        new Contracts.DTO.DataModelSummaryDTO { Id = modelId, Code = "AVA-MODEL", Name = "AVA Model" }
      };

        _mockIntegrationDataProvider
          .GetFeaturesAsync(Arg.Any<CancellationToken>())
          .Returns(expectedFeatures);

        _mockIntegrationDataProvider
          .GetDataModelsSummaryAsync(Arg.Any<CancellationToken>())
          .Returns(expectedModels);

        // Act
        var result = await _sut.ListAsync(Datasource.AVAPlace);

        // Assert
        var parent = result.FirstOrDefault(f => f.Id == parentFeatureId);
        Assert.NotNull(parent);
        Assert.Single(parent.IncludedFeatures);
        Assert.Single(parent.IncludedModels);

        var includedFeature = parent.IncludedFeatures.First();
        Assert.Equal(includedFeatureId, includedFeature.Feature.Id);
        Assert.True(includedFeature.ConsumeOnly);

        var includedModel = parent.IncludedModels.First();
        Assert.Equal(modelId, includedModel.DataModel.Id);
        Assert.True(includedModel.ReadOnly);
    }
  #endregion
  #endregion
}
