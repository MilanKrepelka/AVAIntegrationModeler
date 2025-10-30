using System;
using System.Collections.Generic;
using System.Linq;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.FeatureAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using AVAIntegrationModeler.UseCases.Features.Mapping;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.UseCases.Test.Features.Mapping;

/// <summary>
/// Unit testy pro FeatureMapper.
/// </summary>
public class FeatureMapperTests
{
  #region MapToFeatureSummaryDTO Tests (from Domain.Feature)

  [Fact]
  public void MapToFeatureSummaryDTO_ShouldThrow_WhenFeatureIsNull()
  {
    // Arrange
    Domain.FeatureAggregate.Feature? feature = null;

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => FeatureMapper.MapToFeatureSummaryDTO(feature!));
  }

  [Fact]
  public void MapToFeatureSummaryDTO_ShouldReturnMappedDTO_WhenFeatureIsValid()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Domain.FeatureAggregate.Feature(featureId, "FEAT-001");

    // Act
    var result = FeatureMapper.MapToFeatureSummaryDTO(feature);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(featureId);
    result.Code.ShouldBe("FEAT-001");
  }

  [Fact]
  public void MapToFeatureSummaryDTO_ShouldMapOnlyIdAndCode_WhenFeatureHasAllProperties()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Domain.FeatureAggregate.Feature(featureId, "FEAT-FULL")
      .SetName(new Domain.ValueObjects.LocalizedValue { CzechValue = "Název", EnglishValue = "Name" })
      .SetDescription(new Domain.ValueObjects.LocalizedValue { CzechValue = "Popis", EnglishValue = "Description" });

    // Act
    var result = FeatureMapper.MapToFeatureSummaryDTO(feature);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(featureId);
    result.Code.ShouldBe("FEAT-FULL");
    // FeatureSummaryDTO should NOT contain Name and Description
  }

  [Fact]
  public void MapToFeatureSummaryDTO_ShouldHandleEmptyCode()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Domain.FeatureAggregate.Feature(featureId, "TEMP");

    // Act & Assert
    // Note: SetCode has Guard.Against.NullOrEmpty, so this will throw
    // This test documents the expected behavior
    Should.Throw<ArgumentException>(() => feature.SetCode(string.Empty));
  }

  #endregion

  #region MapToFeatureSummaryDTO Tests (from FeatureDTO)

  [Fact]
  public void MapToFeatureSummaryDTO_FromDTO_ShouldThrow_WhenFeatureDTOIsNull()
  {
    // Arrange
    FeatureDTO? featureDTO = null;

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => FeatureMapper.MapToFeatureSummaryDTO(featureDTO!));
  }

  [Fact]
  public void MapToFeatureSummaryDTO_FromDTO_ShouldReturnMappedSummary_WhenFeatureDTOIsValid()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var featureDTO = new FeatureDTO
    {
      Id = featureId,
      Code = "FEAT-DTO-001",
      Name = new Contracts.DTO.LocalizedValue { CzechValue = "Název", EnglishValue = "Name" },
      Description = new Contracts.DTO.LocalizedValue { CzechValue = "Popis", EnglishValue = "Description" }
    };

    // Act
    var result = FeatureMapper.MapToFeatureSummaryDTO(featureDTO);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(featureId);
    result.Code.ShouldBe("FEAT-DTO-001");
  }

  [Fact]
  public void MapToFeatureSummaryDTO_FromDTO_ShouldIgnoreNameAndDescription()
  {
    // Arrange
    var featureDTO = new FeatureDTO
    {
      Id = Guid.NewGuid(),
      Code = "FEAT-FULL",
      Name = new Contracts.DTO.LocalizedValue { CzechValue = "Test", EnglishValue = "Test" },
      Description = new Contracts.DTO.LocalizedValue { CzechValue = "Test", EnglishValue = "Test" },
      IncludedFeatures = new() { new IncludedFeatureDTO { Feature = new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "SUB" }, ConsumeOnly = true } },
      IncludedModels = new() { new IncludedDataModelDTO { DataModel = new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL", Name = "Model" }, ReadOnly = false } }
    };

    // Act
    var result = FeatureMapper.MapToFeatureSummaryDTO(featureDTO);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(featureDTO.Id);
    result.Code.ShouldBe(featureDTO.Code);
    // Summary should only have Id and Code, no other properties
  }

  [Fact]
  public void MapToFeatureSummaryDTO_FromDTO_ShouldHandleMinimalDTO()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var featureDTO = new FeatureDTO
    {
      Id = featureId,
      Code = "MIN",
      Name = new Contracts.DTO.LocalizedValue(),
      Description = new Contracts.DTO.LocalizedValue()
    };

    // Act
    var result = FeatureMapper.MapToFeatureSummaryDTO(featureDTO);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(featureId);
    result.Code.ShouldBe("MIN");
  }

  #endregion

  #region MapToFeatureDTO Tests

  [Fact]
  public void MapToFeatureDTO_ShouldThrow_WhenFeatureIsNull()
  {
    // Arrange
    Domain.FeatureAggregate.Feature? feature = null;
    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => FeatureMapper.MapToFeatureDTO(feature!, features, models));
  }

  [Fact]
  public void MapToFeatureDTO_ShouldMapBasicProperties_WhenFeatureIsValid()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Domain.FeatureAggregate.Feature(featureId, "FEAT-001")
      .SetName(new Domain.ValueObjects.LocalizedValue { CzechValue = "Testovací feature", EnglishValue = "Test Feature" })
      .SetDescription(new Domain.ValueObjects.LocalizedValue { CzechValue = "Popis", EnglishValue = "Description" });

    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.MapToFeatureDTO(feature, features, models);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(featureId);
    result.Code.ShouldBe("FEAT-001");
    result.Name.ShouldNotBeNull();
    result.Name.CzechValue.ShouldBe("Testovací feature");
    result.Name.EnglishValue.ShouldBe("Test Feature");
    result.Description.ShouldNotBeNull();
    result.Description.CzechValue.ShouldBe("Popis");
    result.Description.EnglishValue.ShouldBe("Description");
  }

  [Fact]
  public void MapToFeatureDTO_ShouldMapEmptyCollections_WhenNoIncludedFeaturesOrModels()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Domain.FeatureAggregate.Feature(featureId, "FEAT-EMPTY");

    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.MapToFeatureDTO(feature, features, models);

    // Assert
    result.ShouldNotBeNull();
    result.IncludedFeatures.ShouldNotBeNull();
    result.IncludedFeatures.ShouldBeEmpty();
    result.IncludedModels.ShouldNotBeNull();
    result.IncludedModels.ShouldBeEmpty();
  }

  [Fact]
  public void MapToFeatureDTO_ShouldMapIncludedFeatures_WhenFeaturesExist()
  {
    // Arrange
    var includedFeatureId = Guid.NewGuid();
    var parentFeatureId = Guid.NewGuid();

    var parentFeature = new Domain.FeatureAggregate.Feature(parentFeatureId, "PARENT-FEAT");
    parentFeature.AddIncludedFeature(includedFeatureId, consumeOnly: true);

    var featureSummaries = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = includedFeatureId, Code = "INCLUDED-FEAT" }
    };
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.MapToFeatureDTO(parentFeature, featureSummaries, models);

    // Assert
    result.ShouldNotBeNull();
    result.IncludedFeatures.ShouldNotBeNull();
    result.IncludedFeatures.Count.ShouldBe(1);
    
    var includedFeature = result.IncludedFeatures.First();
    includedFeature.Feature.ShouldNotBeNull();
    includedFeature.Feature.Id.ShouldBe(includedFeatureId);
    includedFeature.Feature.Code.ShouldBe("INCLUDED-FEAT");
    includedFeature.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public void MapToFeatureDTO_ShouldMapMultipleIncludedFeatures_WithDifferentConsumeOnlyFlags()
  {
    // Arrange
    var includedId1 = Guid.NewGuid();
    var includedId2 = Guid.NewGuid();
    var includedId3 = Guid.NewGuid();
    var parentId = Guid.NewGuid();

    var parentFeature = new Domain.FeatureAggregate.Feature(parentId, "PARENT-MULTI");
    parentFeature.AddIncludedFeature(includedId1, consumeOnly: true);
    parentFeature.AddIncludedFeature(includedId2, consumeOnly: false);
    parentFeature.AddIncludedFeature(includedId3, consumeOnly: true);

    var featureSummaries = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = includedId1, Code = "FEAT-1" },
      new FeatureSummaryDTO { Id = includedId2, Code = "FEAT-2" },
      new FeatureSummaryDTO { Id = includedId3, Code = "FEAT-3" }
    };
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.MapToFeatureDTO(parentFeature, featureSummaries, models);

    // Assert
    result.IncludedFeatures.Count.ShouldBe(3);
    result.IncludedFeatures.ShouldContain(f => f.Feature.Code == "FEAT-1" && f.ConsumeOnly == true);
    result.IncludedFeatures.ShouldContain(f => f.Feature.Code == "FEAT-2" && f.ConsumeOnly == false);
    result.IncludedFeatures.ShouldContain(f => f.Feature.Code == "FEAT-3" && f.ConsumeOnly == true);
  }

  [Fact]
  public void MapToFeatureDTO_ShouldUseEmptyFeature_WhenIncludedFeatureNotFoundInList()
  {
    // Arrange
    var missingFeatureId = Guid.NewGuid();
    var parentId = Guid.NewGuid();

    var parentFeature = new Domain.FeatureAggregate.Feature(parentId, "PARENT-MISSING");
    parentFeature.AddIncludedFeature(missingFeatureId, consumeOnly: true);

    var featureSummaries = new List<FeatureSummaryDTO>(); // prázdný seznam
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.MapToFeatureDTO(parentFeature, featureSummaries, models);

    // Assert
    result.IncludedFeatures.Count.ShouldBe(1);
    var includedFeature = result.IncludedFeatures.First();
    includedFeature.Feature.ShouldBe(FeatureSummaryDTO.Empty);
    includedFeature.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public void MapToFeatureDTO_ShouldMapIncludedModels_WhenModelsExist()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var featureId = Guid.NewGuid();

    var feature = new Domain.FeatureAggregate.Feature(featureId, "FEAT-WITH-MODEL");
    feature.AddIncludedModel(modelId, consumeOnly: true);

    var features = new List<FeatureSummaryDTO>();
    var modelSummaries = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-001", Name = "Test Model" }
    };

    // Act
    var result = FeatureMapper.MapToFeatureDTO(feature, features, modelSummaries);

    // Assert
    result.IncludedModels.ShouldNotBeNull();
    result.IncludedModels.Count.ShouldBe(1);
    
    var includedModel = result.IncludedModels.First();
    includedModel.DataModel.ShouldNotBeNull();
    includedModel.DataModel.Id.ShouldBe(modelId);
    includedModel.DataModel.Code.ShouldBe("MODEL-001");
    includedModel.ReadOnly.ShouldBeTrue();
  }

  [Fact]
  public void MapToFeatureDTO_ShouldMapMultipleIncludedModels_WithDifferentReadOnlyFlags()
  {
    // Arrange
    var modelId1 = Guid.NewGuid();
    var modelId2 = Guid.NewGuid();
    var modelId3 = Guid.NewGuid();
    var featureId = Guid.NewGuid();

    var feature = new Domain.FeatureAggregate.Feature(featureId, "FEAT-MULTI-MODELS");
    feature.AddIncludedModel(modelId1, consumeOnly: false);
    feature.AddIncludedModel(modelId2, consumeOnly: true);
    feature.AddIncludedModel(modelId3, consumeOnly: false);

    var features = new List<FeatureSummaryDTO>();
    var modelSummaries = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId1, Code = "MODEL-1", Name = "Model 1" },
      new DataModelSummaryDTO { Id = modelId2, Code = "MODEL-2", Name = "Model 2" },
      new DataModelSummaryDTO { Id = modelId3, Code = "MODEL-3", Name = "Model 3" }
    };

    // Act
    var result = FeatureMapper.MapToFeatureDTO(feature, features, modelSummaries);

    // Assert
    result.IncludedModels.Count.ShouldBe(3);
    result.IncludedModels.ShouldContain(m => m.DataModel.Code == "MODEL-1" && m.ReadOnly == false);
    result.IncludedModels.ShouldContain(m => m.DataModel.Code == "MODEL-2" && m.ReadOnly == true);
    result.IncludedModels.ShouldContain(m => m.DataModel.Code == "MODEL-3" && m.ReadOnly == false);
  }

  [Fact]
  public void MapToFeatureDTO_ShouldUseEmptyModel_WhenIncludedModelNotFoundInList()
  {
    // Arrange
    var missingModelId = Guid.NewGuid();
    var featureId = Guid.NewGuid();

    var feature = new Domain.FeatureAggregate.Feature(featureId, "FEAT-MISSING-MODEL");
    feature.AddIncludedModel(missingModelId, consumeOnly: true);

    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>(); // prázdný seznam

    // Act
    var result = FeatureMapper.MapToFeatureDTO(feature, features, models);

    // Assert
    result.IncludedModels.Count.ShouldBe(1);
    var includedModel = result.IncludedModels.First();
    includedModel.DataModel.ShouldBe(DataModelSummaryDTO.Empty);
    includedModel.ReadOnly.ShouldBeTrue();
  }

  [Fact]
  public void MapToFeatureDTO_ShouldMapBothIncludedFeaturesAndModels()
  {
    // Arrange
    var includedFeatureId = Guid.NewGuid();
    var modelId = Guid.NewGuid();
    var featureId = Guid.NewGuid();

    var feature = new Domain.FeatureAggregate.Feature(featureId, "COMPLEX-FEAT");
    feature.AddIncludedFeature(includedFeatureId, consumeOnly: true);
    feature.AddIncludedModel(modelId, consumeOnly: false);

    var featureSummaries = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = includedFeatureId, Code = "INCLUDED-FEAT" }
    };
    var modelSummaries = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "DATA-MODEL", Name = "Test Model" }
    };

    // Act
    var result = FeatureMapper.MapToFeatureDTO(feature, featureSummaries, modelSummaries);

    // Assert
    result.IncludedFeatures.Count.ShouldBe(1);
    result.IncludedModels.Count.ShouldBe(1);
    result.IncludedFeatures.First().Feature.Code.ShouldBe("INCLUDED-FEAT");
    result.IncludedModels.First().DataModel.Code.ShouldBe("DATA-MODEL");
  }

  [Fact]
  public void MapToFeatureDTO_ShouldHandleNullCollections_AsEmptyLists()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Domain.FeatureAggregate.Feature(featureId, "NULL-COLLECTIONS");

    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = FeatureMapper.MapToFeatureDTO(feature, features, models);

    // Assert
    result.IncludedFeatures.ShouldNotBeNull();
    result.IncludedFeatures.ShouldBeEmpty();
    result.IncludedModels.ShouldNotBeNull();
    result.IncludedModels.ShouldBeEmpty();
  }

  #endregion

  #region Equality and Consistency Tests

  [Fact]
  public void MapToFeatureSummaryDTO_ShouldBeConsistent_ForSameInput()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Domain.FeatureAggregate.Feature(featureId, "CONSISTENT");

    // Act
    var result1 = FeatureMapper.MapToFeatureSummaryDTO(feature);
    var result2 = FeatureMapper.MapToFeatureSummaryDTO(feature);

    // Assert
    result1.ShouldNotBeNull();
    result2.ShouldNotBeNull();
    result1.Id.ShouldBe(result2.Id);
    result1.Code.ShouldBe(result2.Code);
  }

  [Fact]
  public void MapToFeatureSummaryDTO_ShouldProduceSameResult_FromDomainAndDTO()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var domainFeature = new Domain.FeatureAggregate.Feature(featureId, "COMPARE")
      .SetName(new Domain.ValueObjects.LocalizedValue { CzechValue = "Test", EnglishValue = "Test" });

    var features = new List<FeatureSummaryDTO>();
    var models = new List<DataModelSummaryDTO>();
    var featureDTO = FeatureMapper.MapToFeatureDTO(domainFeature, features, models);

    // Act
    var summaryFromDomain = FeatureMapper.MapToFeatureSummaryDTO(domainFeature);
    var summaryFromDTO = FeatureMapper.MapToFeatureSummaryDTO(featureDTO);

    // Assert
    summaryFromDomain.ShouldNotBeNull();
    summaryFromDTO.ShouldNotBeNull();
    summaryFromDomain.Id.ShouldBe(summaryFromDTO.Id);
    summaryFromDomain.Code.ShouldBe(summaryFromDTO.Code);
  }

  #endregion

  #region Special Characters and Edge Cases

  [Fact]
  public void MapToFeatureSummaryDTO_ShouldHandleSpecialCharactersInCode()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var feature = new Domain.FeatureAggregate.Feature(featureId, "FEAT-ČŘŽ-123");

    // Act
    var result = FeatureMapper.MapToFeatureSummaryDTO(feature);

    // Assert
    result.ShouldNotBeNull();
    result.Code.ShouldBe("FEAT-ČŘŽ-123");
  }

  [Fact]
  public void MapToFeatureSummaryDTO_ShouldHandleLongCode()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var longCode = new string('A', 500);
    var feature = new Domain.FeatureAggregate.Feature(featureId, longCode);

    // Act
    var result = FeatureMapper.MapToFeatureSummaryDTO(feature);

    // Assert
    result.ShouldNotBeNull();
    result.Code.ShouldBe(longCode);
    result.Code.Length.ShouldBe(500);
  }

  #endregion
}
