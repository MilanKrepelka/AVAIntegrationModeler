using ASOL.DataService.Contracts;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.Contracts.DTO;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.AVAPlaceTests.Mapping;

/// <summary>
/// Unit testy pro AVAPlace.IncludedModelMapper.
/// </summary>
public class IncludedModelMapperTests
{
  #region Null Validation Tests

  [Fact]
  public void IncludedDataModelDTO_ShouldThrow_WhenIncludedModelIsNull()
  {
    // Arrange
    IntegrationFeatureDefinition.IncludedModel? includedModel = null;
    var models = new List<DataModelSummaryDTO>();

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => 
      IncludedModelMapper.IncludedDataModelDTO(includedModel!, models));
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldNotThrow_WhenModelsCollectionIsNull()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString(),
      ReadOnly = true
    };

    // Act & Assert
    Should.NotThrow(() => 
      IncludedModelMapper.IncludedDataModelDTO(includedModel, null!));
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldNotThrow_WhenModelsCollectionIsEmpty()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>();

    // Act & Assert
    Should.NotThrow(() => 
      IncludedModelMapper.IncludedDataModelDTO(includedModel, models));
  }

  #endregion

  #region CodeOrId as GUID Tests

  [Fact]
  public void IncludedDataModelDTO_ShouldMapByGuid_WhenCodeOrIdIsValidGuid()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-001", Name = "Test Model" },
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-002", Name = "Other Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.ShouldNotBeNull();
    result.DataModel.ShouldNotBeNull();
    result.DataModel.Id.ShouldBe(modelId);
    result.DataModel.Code.ShouldBe("MODEL-001");
    result.DataModel.Name.ShouldBe("Test Model");
    result.ReadOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldMapByGuid_WithDifferentGuidFormats()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString("D"), // Standard format
      ReadOnly = false
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-FORMAT", Name = "Format Test" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Id.ShouldBe(modelId);
    result.ReadOnly.ShouldBeFalse();
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldMapByGuid_WithUppercaseGuid()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString().ToUpper(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-UPPER", Name = "Upper Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Id.ShouldBe(modelId);
    result.DataModel.Code.ShouldBe("MODEL-UPPER");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldMapByGuid_WithLowercaseGuid()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString().ToLower(),
      ReadOnly = false
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-LOWER", Name = "Lower Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Id.ShouldBe(modelId);
    result.DataModel.Code.ShouldBe("MODEL-LOWER");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldMapByGuid_WithBracesFormat()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString("B"), // Format with braces
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-BRACES", Name = "Braces Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Id.ShouldBe(modelId);
    result.DataModel.Code.ShouldBe("MODEL-BRACES");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldReturnEmpty_WhenGuidNotFoundInModels()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var nonExistingId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = nonExistingId.ToString(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-001", Name = "Test" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.ShouldNotBeNull();
    result.DataModel.ShouldBe(DataModelSummaryDTO.Empty);
    result.ReadOnly.ShouldBeTrue();
  }

  #endregion

  #region CodeOrId as Code Tests

  [Fact]
  public void IncludedDataModelDTO_ShouldMapByCode_WhenCodeOrIdIsNotValidGuid()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = "MODEL-CODE-001",
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-CODE-001", Name = "First Model" },
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-CODE-002", Name = "Second Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.ShouldNotBeNull();
    result.DataModel.ShouldNotBeNull();
    result.DataModel.Code.ShouldBe("MODEL-CODE-001");
    result.DataModel.Name.ShouldBe("First Model");
    result.ReadOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldMapByCode_WithSpecialCharacters()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = "MODEL-ČŘŽ-ÁÍÉ",
      ReadOnly = false
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-ČŘŽ-ÁÍÉ", Name = "Czech Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Code.ShouldBe("MODEL-ČŘŽ-ÁÍÉ");
    result.DataModel.Name.ShouldBe("Czech Model");
    result.ReadOnly.ShouldBeFalse();
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldReturnEmpty_WhenCodeNotFoundInModels()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = "NON-EXISTING-CODE",
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-001", Name = "Test" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.ShouldNotBeNull();
    result.DataModel.ShouldBe(DataModelSummaryDTO.Empty);
    result.ReadOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldMapByCode_WhenCodeOrIdIsInvalidGuidFormat()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = "not-a-guid-123",
      ReadOnly = false
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "not-a-guid-123", Name = "Invalid GUID Code" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Code.ShouldBe("not-a-guid-123");
    result.DataModel.Name.ShouldBe("Invalid GUID Code");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldMapByCode_WithNumericCode()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = "12345",
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "12345", Name = "Numeric Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Code.ShouldBe("12345");
    result.DataModel.Name.ShouldBe("Numeric Model");
  }

  #endregion

  #region ReadOnly Property Tests

  [Fact]
  public void IncludedDataModelDTO_ShouldMapReadOnly_WhenTrue()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-001", Name = "Test" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.ReadOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldMapReadOnly_WhenFalse()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString(),
      ReadOnly = false
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "MODEL-001", Name = "Test" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.ReadOnly.ShouldBeFalse();
  }

  #endregion

  #region Empty Models Collection Tests

  [Fact]
  public void IncludedDataModelDTO_ShouldReturnEmpty_WhenModelsCollectionIsEmpty()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>();

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.ShouldNotBeNull();
    result.DataModel.ShouldBe(DataModelSummaryDTO.Empty);
    result.ReadOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldReturnEmpty_WhenModelsCollectionIsNull()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString(),
      ReadOnly = false
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, null!);

    // Assert
    result.ShouldNotBeNull();
    result.DataModel.ShouldBe(DataModelSummaryDTO.Empty);
    result.ReadOnly.ShouldBeFalse();
  }

  #endregion

  #region Multiple Models Tests

  [Fact]
  public void IncludedDataModelDTO_ShouldSelectCorrectModel_FromMultipleModels()
  {
    // Arrange
    var targetId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = targetId.ToString(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-001", Name = "First" },
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-002", Name = "Second" },
      new DataModelSummaryDTO { Id = targetId, Code = "MODEL-TARGET", Name = "Target Model" },
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-003", Name = "Third" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Id.ShouldBe(targetId);
    result.DataModel.Code.ShouldBe("MODEL-TARGET");
    result.DataModel.Name.ShouldBe("Target Model");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldReturnFirstMatch_WhenMultipleModelsHaveSameCode()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = "DUPLICATE-CODE",
      ReadOnly = true
    };
    var firstId = Guid.NewGuid();
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = firstId, Code = "DUPLICATE-CODE", Name = "First Duplicate" },
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "DUPLICATE-CODE", Name = "Second Duplicate" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Id.ShouldBe(firstId);
    result.DataModel.Code.ShouldBe("DUPLICATE-CODE");
    result.DataModel.Name.ShouldBe("First Duplicate");
  }

  #endregion

  #region Edge Cases

  [Fact]
  public void IncludedDataModelDTO_ShouldHandleEmptyGuid()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = Guid.Empty.ToString(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.Empty, Code = "EMPTY-GUID-MODEL", Name = "Empty Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Id.ShouldBe(Guid.Empty);
    result.DataModel.Code.ShouldBe("EMPTY-GUID-MODEL");
    result.DataModel.Name.ShouldBe("Empty Model");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldHandleNullCodeOrId()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = null,
      ReadOnly = false
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-001", Name = "Test" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.ShouldNotBeNull();
    result.DataModel.ShouldBe(DataModelSummaryDTO.Empty);
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldHandleEmptyCodeOrId()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = string.Empty,
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = string.Empty, Name = "Empty Code" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Code.ShouldBe(string.Empty);
    result.DataModel.Name.ShouldBe("Empty Code");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldHandleWhitespaceCodeOrId()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = "   ",
      ReadOnly = false
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "   ", Name = "Whitespace Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Code.ShouldBe("   ");
    result.DataModel.Name.ShouldBe("Whitespace Model");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldBeConsistent_ForSameInput()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "CONSISTENT-MODEL", Name = "Consistent" }
    };

    // Act
    var result1 = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);
    var result2 = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result1.DataModel.Id.ShouldBe(result2.DataModel.Id);
    result1.DataModel.Code.ShouldBe(result2.DataModel.Code);
    result1.DataModel.Name.ShouldBe(result2.DataModel.Name);
    result1.ReadOnly.ShouldBe(result2.ReadOnly);
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldHandleLongCode()
  {
    // Arrange
    var longCode = new string('A', 1000);
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = longCode,
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = longCode, Name = "Long Code Model" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Code.ShouldBe(longCode);
    result.DataModel.Code.Length.ShouldBe(1000);
    result.DataModel.Name.ShouldBe("Long Code Model");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldHandleLongName()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var longName = new string('B', 2000);
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = modelId.ToString(),
      ReadOnly = false
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = modelId, Code = "LONG-NAME", Name = longName }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Name.ShouldBe(longName);
    result.DataModel.Name.Length.ShouldBe(2000);
  }

  #endregion

  #region Case Sensitivity Tests

  [Fact]
  public void IncludedDataModelDTO_ShouldBeCaseSensitive_WhenMatchingByCode()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = "MODEL-UPPER",
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "model-upper", Name = "Lowercase" },
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "MODEL-UPPER", Name = "Uppercase" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Code.ShouldBe("MODEL-UPPER");
    result.DataModel.Name.ShouldBe("Uppercase");
  }

  #endregion

  #region Performance and Large Collections Tests

  [Fact]
  public void IncludedDataModelDTO_ShouldHandleLargeModelCollection()
  {
    // Arrange
    var targetId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = targetId.ToString(),
      ReadOnly = true
    };
    
    var models = new List<DataModelSummaryDTO>();
    for (int i = 0; i < 1000; i++)
    {
      models.Add(new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = $"MODEL-{i}", Name = $"Model {i}" });
    }
    models.Add(new DataModelSummaryDTO { Id = targetId, Code = "TARGET-MODEL", Name = "Target" });

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Id.ShouldBe(targetId);
    result.DataModel.Code.ShouldBe("TARGET-MODEL");
    result.DataModel.Name.ShouldBe("Target");
  }

  #endregion

  #region Guid TryParse Edge Cases

  [Fact]
  public void IncludedDataModelDTO_ShouldFallbackToCode_WhenCodeOrIdLooksLikeGuidButIsNot()
  {
    // Arrange
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = "12345678-1234-5678-1234-567812345678X", // Invalid GUID
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = "12345678-1234-5678-1234-567812345678X", Name = "Almost GUID" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Code.ShouldBe("12345678-1234-5678-1234-567812345678X");
    result.DataModel.Name.ShouldBe("Almost GUID");
  }

  [Fact]
  public void IncludedDataModelDTO_ShouldPreferGuidMatch_OverCodeMatch()
  {
    // Arrange
    var targetId = Guid.NewGuid();
    var includedModel = new IntegrationFeatureDefinition.IncludedModel
    {
      CodeOrId = targetId.ToString(),
      ReadOnly = true
    };
    var models = new List<DataModelSummaryDTO>
    {
      new DataModelSummaryDTO { Id = Guid.NewGuid(), Code = targetId.ToString(), Name = "Code Match" },
      new DataModelSummaryDTO { Id = targetId, Code = "GUID-MATCH", Name = "GUID Match" }
    };

    // Act
    var result = IncludedModelMapper.IncludedDataModelDTO(includedModel, models);

    // Assert
    result.DataModel.Id.ShouldBe(targetId);
    result.DataModel.Code.ShouldBe("GUID-MATCH");
    result.DataModel.Name.ShouldBe("GUID Match");
  }

  #endregion
}
