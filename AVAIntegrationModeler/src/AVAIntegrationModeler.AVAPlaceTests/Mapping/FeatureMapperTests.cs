using System;
using ASOL.Core.Localization;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.AVAPlace.Tests.Mapping;

/// <summary>
/// Unit testy pro <see cref="FeatureMapper"/>.
/// </summary>
public class FeatureMapperTests
{
  #region FeatureSummaryDTO Tests

  [Fact]
  public void FeatureSummaryDTO_ValidInput_ReturnsCorrectMapping()
  {
    // Arrange
    var expectedId = Guid.NewGuid();
    var expectedCode = "FEATURE_001";

    var integrationFeatureSummary = new IntegrationFeatureSummary
    {
      Id = expectedId.ToString(),
      Code = expectedCode
    };

    // Act
    var result = FeatureMapper.FeatureSummaryDTO(integrationFeatureSummary);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedId, result.Id);
    Assert.Equal(expectedCode, result.Code);
  }

  [Fact]
  public void FeatureSummaryDTO_NullInput_ThrowsArgumentNullException()
  {
    // Arrange
    IntegrationFeatureSummary? nullSummary = null;

    // Act & Assert
    var exception = Assert.Throws<ArgumentNullException>(
        () => FeatureMapper.FeatureSummaryDTO(nullSummary!)
    );

    Assert.Contains("integrationDefenitionSummary", exception.Message);
  }

  [Fact]
  public void FeatureSummaryDTO_NullId_ThrowsArgumentException()
  {
    // Arrange
    var integrationFeatureSummary = new IntegrationFeatureSummary
    {
      Id = null!,
      Code = "FEATURE_001"
    };

    // Act & Assert
    var exception = Assert.Throws<ArgumentNullException>(
        () => FeatureMapper.FeatureSummaryDTO(integrationFeatureSummary)
    );

    Assert.Contains("Id", exception.Message);
  }

  [Fact]
  public void FeatureSummaryDTO_EmptyId_ThrowsArgumentException()
  {
    // Arrange
    var integrationFeatureSummary = new IntegrationFeatureSummary
    {
      Id = string.Empty,
      Code = "FEATURE_001"
    };

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(
        () => FeatureMapper.FeatureSummaryDTO(integrationFeatureSummary)
    );

    Assert.Contains("Id", exception.Message);
  }

  [Fact]
  public void FeatureSummaryDTO_InvalidGuidFormat_ThrowsFormatException()
  {
    // Arrange
    var integrationFeatureSummary = new IntegrationFeatureSummary
    {
      Id = "invalid-guid-format",
      Code = "FEATURE_001"
    };

    // Act & Assert
    Assert.Throws<FormatException>(
        () => FeatureMapper.FeatureSummaryDTO(integrationFeatureSummary)
    );
  }

  [Fact]
  public void FeatureSummaryDTO_EmptyCode_MapsCorrectly()
  {
    // Arrange
    var expectedId = Guid.NewGuid();

    var integrationFeatureSummary = new IntegrationFeatureSummary
    {
      Id = expectedId.ToString(),
      Code = string.Empty
    };

    // Act
    var result = FeatureMapper.FeatureSummaryDTO(integrationFeatureSummary);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedId, result.Id);
    Assert.Equal(string.Empty, result.Code);
  }

  [Fact]
  public void FeatureSummaryDTO_NullCode_MapsCorrectly()
  {
    // Arrange
    var expectedId = Guid.NewGuid();

    var integrationFeatureSummary = new IntegrationFeatureSummary
    {
      Id = expectedId.ToString(),
      Code = null!
    };

    // Act
    var result = FeatureMapper.FeatureSummaryDTO(integrationFeatureSummary);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(expectedId, result.Id);
    Assert.Null(result.Code);
  }

  [Theory]
  [InlineData("00000000-0000-0000-0000-000000000000")] // Empty GUID
  [InlineData("12345678-1234-1234-1234-123456789012")]
  [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
  public void FeatureSummaryDTO_VariousValidGuids_MapsCorrectly(string guidString)
  {
    // Arrange
    var expectedId = Guid.Parse(guidString);

    var integrationFeatureSummary = new IntegrationFeatureSummary
    {
      Id = guidString,
      Code = "TEST"
    };

    // Act
    var result = FeatureMapper.FeatureSummaryDTO(integrationFeatureSummary);

    // Assert
    Assert.Equal(expectedId, result.Id);
  }

  [Theory]
  [InlineData("SIMPLE_CODE")]
  [InlineData("CODE-WITH-DASHES")]
  [InlineData("Code_With_123")]
  [InlineData("ÚnicodéČródě")]
  public void FeatureSummaryDTO_VariousCodeFormats_MapsCorrectly(string code)
  {
    // Arrange
    var id = Guid.NewGuid();

    var integrationFeatureSummary = new IntegrationFeatureSummary
    {
      Id = id.ToString(),
      Code = code
    };

    // Act
    var result = FeatureMapper.FeatureSummaryDTO(integrationFeatureSummary);

    // Assert
    Assert.Equal(code, result.Code);
  }

  #endregion

  #region FeatureDTO(IntegrationFeatureModel) Tests - Basic

  [Fact]
  public void FeatureDTO_FromModel_ShouldThrow_WhenIntegrationFeatureModelIsNull()
  {
    // Arrange
    IntegrationFeatureModel? integrationFeatureModel = null;
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act & Assert
    Should.Throw<ArgumentNullException>(() =>
      FeatureMapper.FeatureDTO(integrationFeatureModel!, features, models));
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldThrow_WhenIdIsNull()
  {
    // Arrange
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = null!,
      Code = "FEAT-001"
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act & Assert
    Should.Throw<ArgumentException>(() =>
      FeatureMapper.FeatureDTO(integrationFeatureModel, features, models));
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldThrow_WhenIdIsEmpty()
  {
    // Arrange
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = string.Empty,
      Code = "FEAT-001"
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act & Assert
    Should.Throw<ArgumentException>(() =>
      FeatureMapper.FeatureDTO(integrationFeatureModel, features, models));
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldThrow_WhenIdIsInvalidGuid()
  {
    // Arrange
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = "not-a-valid-guid",
      Code = "FEAT-001"
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act & Assert
    Should.Throw<FormatException>(() =>
      FeatureMapper.FeatureDTO(integrationFeatureModel, features, models));
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldMapBasicProperties_WhenInputIsValid()
  {
    // Arrange
    var expectedId = Guid.NewGuid();
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = expectedId.ToString(),
      Code = "FEAT-MODEL-001",
      Name = new LocalizedValue<string>
      {
        Values = new List<LocalizedValueItem<string>>
        {
          new LocalizedValueItem<string> { Locale = "cs-CZ", Value = "Model CZ" },
          new LocalizedValueItem<string> { Locale = "en-US", Value = "Model EN" }
        }
      },
      Description = new LocalizedValue<string>
      {
        Values = new List<LocalizedValueItem<string>>
        {
          new LocalizedValueItem<string> { Locale = "cs-CZ", Value = "Popis Model CZ" },
          new LocalizedValueItem<string> { Locale = "en-US", Value = "Description Model EN" }
        }
      }
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(expectedId);
    result.Code.ShouldBe("FEAT-MODEL-001");
    result.Name.ShouldNotBeNull();
    result.Name.CzechValue.ShouldBe("Model CZ");
    result.Name.EnglishValue.ShouldBe("Model EN");
    result.Description.ShouldNotBeNull();
    result.Description.CzechValue.ShouldBe("Popis Model CZ");
    result.Description.EnglishValue.ShouldBe("Description Model EN");
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldHandleNullName()
  {
    // Arrange
    var expectedId = Guid.NewGuid();
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = expectedId.ToString(),
      Code = "FEAT-NULL-NAME",
      Name = null,
      Description = new LocalizedValue<string>
      {
        Values = new List<LocalizedValueItem<string>>
        {
          new LocalizedValueItem<string> { Locale = "cs-CZ", Value = "Popis" },
          new LocalizedValueItem<string> { Locale = "en-US", Value = "Description" }
        }
      }
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.ShouldNotBeNull();
    result.Name.ShouldNotBeNull();
    result.Name.CzechValue.ShouldBe(string.Empty);
    result.Name.EnglishValue.ShouldBe(string.Empty);
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldHandleNullDescription()
  {
    // Arrange
    var expectedId = Guid.NewGuid();
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = expectedId.ToString(),
      Code = "FEAT-NULL-DESC",
      Name = new LocalizedValue<string>
      {
        Values = new List<LocalizedValueItem<string>>
        {
          new LocalizedValueItem<string> { Locale = "cs-CZ", Value = "Název" },
          new LocalizedValueItem<string> { Locale = "en-US", Value = "Name" }
        }
      },
      Description = null
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.ShouldNotBeNull();
    result.Description.ShouldNotBeNull();
    result.Description.CzechValue.ShouldBe(string.Empty);
    result.Description.EnglishValue.ShouldBe(string.Empty);
  }

  #endregion

  #region FeatureDTO(IntegrationFeatureModel) - IncludedFeatures Tests

  [Fact]
  public void FeatureDTO_FromModel_ShouldMapSingleIncludedFeature_ByGuid()
  {
    // Arrange
    var parentId = Guid.NewGuid();
    var includedFeatureId = Guid.NewGuid();

    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = parentId.ToString(),
      Code = "PARENT-FEAT",
      IncludedFeatures = new List<IntegrationFeatureModel.IncludedFeatureModel>
      {
        new IntegrationFeatureModel.IncludedFeatureModel
        {
          FeatureId = includedFeatureId.ToString(),
          ConsumeOnly = true
        }
      }
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = includedFeatureId, Code = "INCLUDED-FEAT-001" }
    };
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.ShouldNotBeNull();
    result.IncludedFeatures.ShouldNotBeNull();
    result.IncludedFeatures.Count.ShouldBe(1);

    var includedFeature = result.IncludedFeatures.First();
    includedFeature.Feature.ShouldNotBeNull();
    includedFeature.Feature.Id.ShouldBe(includedFeatureId);
    includedFeature.Feature.Code.ShouldBe("INCLUDED-FEAT-001");
    includedFeature.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldMapSingleIncludedFeature_ByCode()
  {
    // Arrange
    var parentId = Guid.NewGuid();
    var includedFeatureId = Guid.NewGuid();

    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = parentId.ToString(),
      Code = "PARENT-FEAT",
      IncludedFeatures = new List<IntegrationFeatureModel.IncludedFeatureModel>
      {
        new IntegrationFeatureModel.IncludedFeatureModel
        {
          FeatureId = "INCLUDED-BY-CODE",
          ConsumeOnly = false
        }
      }
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = includedFeatureId, Code = "INCLUDED-BY-CODE" }
    };
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.IncludedFeatures.Count.ShouldBe(1);
    result.IncludedFeatures.First().Feature.Code.ShouldBe("INCLUDED-BY-CODE");
    result.IncludedFeatures.First().ConsumeOnly.ShouldBeFalse();
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldMapMultipleIncludedFeatures()
  {
    // Arrange
    var parentId = Guid.NewGuid();
    var includedId1 = Guid.NewGuid();
    var includedId2 = Guid.NewGuid();
    var includedId3 = Guid.NewGuid();

    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = parentId.ToString(),
      Code = "PARENT-MULTI",
      IncludedFeatures = new List<IntegrationFeatureModel.IncludedFeatureModel>
      {
        new IntegrationFeatureModel.IncludedFeatureModel { FeatureId = includedId1.ToString(), ConsumeOnly = true },
        new IntegrationFeatureModel.IncludedFeatureModel { FeatureId = includedId2.ToString(), ConsumeOnly = false },
        new IntegrationFeatureModel.IncludedFeatureModel { FeatureId = includedId3.ToString(), ConsumeOnly = true }
      }
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = includedId1, Code = "FEAT-1" },
      new FeatureSummaryDTO { Id = includedId2, Code = "FEAT-2" },
      new FeatureSummaryDTO { Id = includedId3, Code = "FEAT-3" }
    };
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.IncludedFeatures.Count.ShouldBe(3);
    result.IncludedFeatures.ShouldContain(f => f.Feature.Code == "FEAT-1" && f.ConsumeOnly == true);
    result.IncludedFeatures.ShouldContain(f => f.Feature.Code == "FEAT-2" && f.ConsumeOnly == false);
    result.IncludedFeatures.ShouldContain(f => f.Feature.Code == "FEAT-3" && f.ConsumeOnly == true);
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldHandleNullIncludedFeaturesCollection()
  {
    // Arrange
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = Guid.NewGuid().ToString(),
      Code = "NULL-FEATURES",
      IncludedFeatures = null
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.IncludedFeatures.ShouldNotBeNull();
    result.IncludedFeatures.ShouldBeEmpty();
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldUseEmptyFeature_WhenIncludedFeatureNotFound()
  {
    // Arrange
    var includedFeatureId = Guid.NewGuid();
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = Guid.NewGuid().ToString(),
      Code = "MISSING-FEATURE",
      IncludedFeatures = new List<IntegrationFeatureModel.IncludedFeatureModel>
      {
        new IntegrationFeatureModel.IncludedFeatureModel
        {
          FeatureId = includedFeatureId.ToString(),
          ConsumeOnly = true
        }
      }
    };
    var features = new List<FeatureSummaryDTO>(); // Empty list
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.IncludedFeatures.Count.ShouldBe(1);
    result.IncludedFeatures.First().Feature.ShouldBe(FeatureSummaryDTO.Empty);
    result.IncludedFeatures.First().ConsumeOnly.ShouldBeTrue();
  }

  #endregion

  #region FeatureDTO(IntegrationFeatureModel) - IncludedModels Tests

  [Fact]
  public void FeatureDTO_FromModel_ShouldMapSingleIncludedModel_ByGuid()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var modelId = Guid.NewGuid();

    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = featureId.ToString(),
      Code = "FEAT-WITH-MODEL",
      IncludedModels = new List<IntegrationFeatureModel.IncludedDataModel>
      {
        new IntegrationFeatureModel.IncludedDataModel
        {
          ModelId = modelId.ToString(),
          ReadOnly = true
        }
      }
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-001", Name = "Test Model" }
    };

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.ShouldNotBeNull();
    result.IncludedModels.ShouldNotBeNull();
    result.IncludedModels.Count.ShouldBe(1);

    var includedModel = result.IncludedModels.First();
    includedModel.DataModel.ShouldNotBeNull();
    includedModel.DataModel.Id.ShouldBe(modelId);
    includedModel.DataModel.Code.ShouldBe("MODEL-001");
    includedModel.DataModel.Name.ShouldBe("Test Model");
    includedModel.ReadOnly.ShouldBeTrue();
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldMapSingleIncludedModel_ByCode()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var modelId = Guid.NewGuid();

    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = featureId.ToString(),
      Code = "FEAT-WITH-MODEL",
      IncludedModels = new List<IntegrationFeatureModel.IncludedDataModel>
      {
        new IntegrationFeatureModel.IncludedDataModel
        {
          ModelId = "MODEL-BY-CODE",
          ReadOnly = false
        }
      }
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-BY-CODE", Name = "Code Model" }
    };

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.IncludedModels.Count.ShouldBe(1);
    result.IncludedModels.First().DataModel.Code.ShouldBe("MODEL-BY-CODE");
    result.IncludedModels.First().ReadOnly.ShouldBeFalse();
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldMapMultipleIncludedModels()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var modelId1 = Guid.NewGuid();
    var modelId2 = Guid.NewGuid();
    var modelId3 = Guid.NewGuid();

    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = featureId.ToString(),
      Code = "FEAT-MULTI-MODELS",
      IncludedModels = new List<IntegrationFeatureModel.IncludedDataModel>
      {
        new IntegrationFeatureModel.IncludedDataModel { ModelId = modelId1.ToString(), ReadOnly = false },
        new IntegrationFeatureModel.IncludedDataModel { ModelId = modelId2.ToString(), ReadOnly = true },
        new IntegrationFeatureModel.IncludedDataModel { ModelId = modelId3.ToString(), ReadOnly = false }
      }
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId1, Code = "MODEL-1", Name = "Model 1" },
      new DataModelSummaryDTO { Id = modelId2, Code = "MODEL-2", Name = "Model 2" },
      new DataModelSummaryDTO { Id = modelId3, Code = "MODEL-3", Name = "Model 3" }
    };

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.IncludedModels.Count.ShouldBe(3);
    result.IncludedModels.ShouldContain(m => m.DataModel.Code == "MODEL-1" && m.ReadOnly == false);
    result.IncludedModels.ShouldContain(m => m.DataModel.Code == "MODEL-2" && m.ReadOnly == true);
    result.IncludedModels.ShouldContain(m => m.DataModel.Code == "MODEL-3" && m.ReadOnly == false);
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldHandleNullIncludedModelsCollection()
  {
    // Arrange
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = Guid.NewGuid().ToString(),
      Code = "NULL-MODELS",
      IncludedModels = null
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.IncludedModels.ShouldNotBeNull();
    result.IncludedModels.ShouldBeEmpty();
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldUseEmptyModel_WhenIncludedModelNotFound()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = Guid.NewGuid().ToString(),
      Code = "MISSING-MODEL",
      IncludedModels = new List<IntegrationFeatureModel.IncludedDataModel>
      {
        new IntegrationFeatureModel.IncludedDataModel
        {
          ModelId = modelId.ToString(),
          ReadOnly = true
        }
      }
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>(); // Empty list

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.IncludedModels.Count.ShouldBe(1);
    result.IncludedModels.First().DataModel.ShouldBe(DataModelSummaryDTO.Empty);
    result.IncludedModels.First().ReadOnly.ShouldBeTrue();
  }

  #endregion

  #region FeatureDTO(IntegrationFeatureModel) - Complex Scenarios

  [Fact]
  public void FeatureDTO_FromModel_ShouldMapBothIncludedFeaturesAndModels()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeatureId = Guid.NewGuid();
    var modelId = Guid.NewGuid();

    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = featureId.ToString(),
      Code = "COMPLEX-FEAT",
      Name = new LocalizedValue<string>
      {
        Values = new List<LocalizedValueItem<string>>
        {
          new LocalizedValueItem<string> { Locale = "cs-CZ", Value = "Komplexní" },
          new LocalizedValueItem<string> { Locale = "en-US", Value = "Complex" }
        }
      },
      IncludedFeatures = new List<IntegrationFeatureModel.IncludedFeatureModel>
      {
        new IntegrationFeatureModel.IncludedFeatureModel { FeatureId = includedFeatureId.ToString(), ConsumeOnly = true }
      },
      IncludedModels = new List<IntegrationFeatureModel.IncludedDataModel>
      {
        new IntegrationFeatureModel.IncludedDataModel { ModelId = modelId.ToString(), ReadOnly = false }
      }
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = includedFeatureId, Code = "INCLUDED-FEAT" }
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "DATA-MODEL", Name = "Data Model" }
    };

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.ShouldNotBeNull();
    result.IncludedFeatures.Count.ShouldBe(1);
    result.IncludedModels.Count.ShouldBe(1);
    result.IncludedFeatures.First().Feature.Code.ShouldBe("INCLUDED-FEAT");
    result.IncludedModels.First().DataModel.Code.ShouldBe("DATA-MODEL");
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldHandleNullFeaturesCollection()
  {
    // Arrange
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = Guid.NewGuid().ToString(),
      Code = "NULL-FEAT-COLLECTION",
      IncludedFeatures = new List<IntegrationFeatureModel.IncludedFeatureModel>
      {
        new IntegrationFeatureModel.IncludedFeatureModel { FeatureId = Guid.NewGuid().ToString(), ConsumeOnly = true }
      }
    };
    var models = new List<DataModelSummaryDTO>();

    // Act
    Should.Throw<ArgumentNullException>(() =>
      FeatureMapper.FeatureDTO(integrationFeatureModel, null!, models));
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldHandleNullModelsCollection()
  {
    // Arrange
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = Guid.NewGuid().ToString(),
      Code = "NULL-MODEL-COLLECTION",
      IncludedModels = new List<IntegrationFeatureModel.IncludedDataModel>
      {
        new IntegrationFeatureModel.IncludedDataModel { ModelId = Guid.NewGuid().ToString(), ReadOnly = true }
      }
    };
    var features = new List<FeatureSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, null!);

    // Assert
    result.IncludedModels.Count.ShouldBe(1);
    result.IncludedModels.First().DataModel.ShouldBe(DataModelSummaryDTO.Empty);
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldHandleSpecialCharactersInCode()
  {
    // Arrange
    var expectedId = Guid.NewGuid();
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = expectedId.ToString(),
      Code = "FEAT-ČŘŽ-ÁÍÉ-999"
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result.ShouldNotBeNull();
    result.Code.ShouldBe("FEAT-ČŘŽ-ÁÍÉ-999");
  }

  [Fact]
  public void FeatureDTO_FromModel_ShouldBeConsistent_ForSameInput()
  {
    // Arrange
    var expectedId = Guid.NewGuid();
    var integrationFeatureModel = new IntegrationFeatureModel
    {
      Id = expectedId.ToString(),
      Code = "CONSISTENT-001",
      Name = new LocalizedValue<string>
      {
        Values = new List<LocalizedValueItem<string>>
        {
          new LocalizedValueItem<string> { Locale = "cs-CZ", Value = "Test" },
          new LocalizedValueItem<string> { Locale = "en-US", Value = "Test" }
        }
      }
    };
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result1 = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);
    var result2 = FeatureMapper.FeatureDTO(integrationFeatureModel, features, models);

    // Assert
    result1.ShouldNotBeNull();
    result2.ShouldNotBeNull();
    result1.Id.ShouldBe(result2.Id);
    result1.Code.ShouldBe(result2.Code);
    result1.Name.CzechValue.ShouldBe(result2.Name.CzechValue);
  }

  #endregion

}
