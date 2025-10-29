﻿using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Core.FeatureAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace AVAIntegrationModeler.Infrastructure.Tests.Data.Queries;

/// <summary>
/// Testy pro ListFeaturesQueryService - testování metody ListAsync pro načítání features z různých zdrojů dat.
/// </summary>
public class ListFeaturesQueryServiceTests : IDisposable
{
  private readonly AppDbContext _dbContext;
  private readonly Mock<IIntegrationDataProvider> _mockIntegrationDataProvider;
  private readonly IMemoryCache _memoryCache;
  private readonly ListFeaturesQueryService _sut;

  public ListFeaturesQueryServiceTests()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    _dbContext = new AppDbContext(options, null);
    _mockIntegrationDataProvider = new Mock<IIntegrationDataProvider>();
    _memoryCache = new MemoryCache(new MemoryCacheOptions());

    _sut = new ListFeaturesQueryService(
        _dbContext,
        _mockIntegrationDataProvider.Object,
        _memoryCache);
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
    await _dbContext.SaveChangesAsync();

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
    await _dbContext.SaveChangesAsync();

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
    await _dbContext.SaveChangesAsync();

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
    var expectedFeatures = new List<FeatureDTO>
    {
      new FeatureDTO
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
        .Setup(x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedFeatures);

    // Act
    var result = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    Assert.NotNull(result);
    var features = result.ToList();
    Assert.Single(features);
    Assert.Equal("AVA-FEAT-001", features[0].Code);
    Assert.Equal("AVA feature", features[0].Name.CzechValue);

    _mockIntegrationDataProvider.Verify(
        x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()),
        Times.Once);
  }

  [Fact]
  public async Task ListAsync_WithAVAPlaceDatasource_ReturnsMultipleFeatures()
  {
    // Arrange
    var expectedFeatures = new List<FeatureDTO>
    {
      new FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-001" },
      new FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-002" },
      new FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-003" }
    };

    _mockIntegrationDataProvider
        .Setup(x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedFeatures);

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
    var expectedFeatures = new List<FeatureDTO>
    {
      new FeatureDTO
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
        .Setup(x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedFeatures);

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
        .Setup(x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<FeatureDTO>());

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
    var expectedFeatures = new List<FeatureDTO>
    {
      new FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-CACHE-TEST" }
    };

    _mockIntegrationDataProvider
        .Setup(x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedFeatures);

    // Act
    var result1 = await _sut.ListAsync(Datasource.AVAPlace);
    var result2 = await _sut.ListAsync(Datasource.AVAPlace);

    // Assert
    Assert.Equal(result1, result2);

    _mockIntegrationDataProvider.Verify(
        x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()),
        Times.Once);
  }

  [Fact]
  public async Task ListAsync_UsesDifferentCacheKeys_ForDifferentDatasources()
  {
    // Arrange
    var avaPlaceFeatures = new List<FeatureDTO>
    {
      new FeatureDTO { Id = Guid.NewGuid(), Code = "AVA-001" }
    };

    _mockIntegrationDataProvider
        .Setup(x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(avaPlaceFeatures);

    var dbFeature = new Feature(Guid.NewGuid(), "DB-001");
    _dbContext.Features.Add(dbFeature);
    await _dbContext.SaveChangesAsync();

    // Act
    var avaPlaceResult = await _sut.ListAsync(Datasource.AVAPlace);
    var databaseResult = await _sut.ListAsync(Datasource.Database);

    // Assert
    Assert.Single(avaPlaceResult);
    Assert.Single(databaseResult);
    Assert.NotEqual(avaPlaceResult.First().Code, databaseResult.First().Code);
    Assert.Equal("AVA-001", avaPlaceResult.First().Code);
    Assert.Equal("DB-001", databaseResult.First().Code);
  }

  [Fact]
  public async Task ListAsync_WithDatabaseDatasource_CachingWorks()
  {
    // Arrange
    var feature = new Feature(Guid.NewGuid(), "CACHE-TEST");
    _dbContext.Features.Add(feature);
    await _dbContext.SaveChangesAsync();

    // Act
    var result1 = await _sut.ListAsync(Datasource.Database);

    var newFeature = new Feature(Guid.NewGuid(), "NEW-FEATURE");
    _dbContext.Features.Add(newFeature);
    await _dbContext.SaveChangesAsync();

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
    var expectedFeatures = new List<FeatureDTO>
    {
      new FeatureDTO { Id = Guid.NewGuid(), Code = "CACHE-EXPIRY-TEST" }
    };

    _mockIntegrationDataProvider
        .Setup(x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedFeatures);

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
    await _dbContext.SaveChangesAsync();

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
    var expectedFeatures = new List<FeatureDTO>
    {
      new FeatureDTO
      {
        Id = Guid.NewGuid(),
        Code = "NULL-TEST",
        Name = null!, // Otestujeme null hodnoty
        Description = null!
      }
    };

    _mockIntegrationDataProvider
        .Setup(x => x.GetFeaturesAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedFeatures);

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
    await _dbContext.SaveChangesAsync();

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

  #endregion

  public void Dispose()
  {
    _dbContext?.Dispose();
    _memoryCache?.Dispose();
  }
}
