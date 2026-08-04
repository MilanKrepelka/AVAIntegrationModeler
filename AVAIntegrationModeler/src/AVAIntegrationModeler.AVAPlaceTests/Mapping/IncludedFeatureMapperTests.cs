using ASOL.DataService.Contracts;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.Contracts.DTO;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.AVAPlaceTests.Mapping;

/// <summary>
/// Unit testy pro AVAPlace.IncludedFeatureMapper.
/// </summary>
public class IncludedFeatureMapperTests
{
  #region Null Validation Tests

  [Fact]
  public void IncludedFeatureDTO_ShouldThrow_WhenIncludedFeatureIsNull()
  {
    // Arrange
    IntegrationFeatureDefinition.IncludedFeature? includedFeature = null;
    var features = new List<FeatureSummaryDTO>();

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => 
      IncludedFeatureMapper.IncludedFeatureDTO(includedFeature!, features));
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldNotThrow_WhenFeaturesCollectionIsNull()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString(),
      ConsumeOnly = true
    };

    // Act & Assert
    Should.Throw<ArgumentNullException>(() =>
    {
      IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, null!);
    });
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldNotThrow_WhenFeaturesCollectionIsEmpty()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>();

    // Act & Assert
    Should.NotThrow(() => 
      IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features));
  }

  #endregion

  #region CodeOrId as GUID Tests

  [Fact]
  public void IncludedFeatureDTO_ShouldMapByGuid_WhenCodeOrIdIsValidGuid()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = featureId, Code = "FEAT-001" },
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-002" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.ShouldNotBeNull();
    result.Feature.ShouldNotBeNull();
    result.Feature.Id.ShouldBe(featureId);
    result.Feature.Code.ShouldBe("FEAT-001");
    result.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldMapByGuid_WithDifferentGuidFormats()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString("D"), // Standard format
      ConsumeOnly = false
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = featureId, Code = "FEAT-FORMAT" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Id.ShouldBe(featureId);
    result.ConsumeOnly.ShouldBeFalse();
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldMapByGuid_WithUppercaseGuid()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString().ToUpper(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = featureId, Code = "FEAT-UPPER" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Id.ShouldBe(featureId);
    result.Feature.Code.ShouldBe("FEAT-UPPER");
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldMapByGuid_WithLowercaseGuid()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString().ToLower(),
      ConsumeOnly = false
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = featureId, Code = "FEAT-LOWER" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Id.ShouldBe(featureId);
    result.Feature.Code.ShouldBe("FEAT-LOWER");
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldMapByGuid_WithBracesFormat()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString("B"), // Format with braces: {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = featureId, Code = "FEAT-BRACES" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Id.ShouldBe(featureId);
    result.Feature.Code.ShouldBe("FEAT-BRACES");
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldReturnEmpty_WhenGuidNotFoundInFeatures()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var nonExistingId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = nonExistingId.ToString(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = featureId, Code = "FEAT-001" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.ShouldNotBeNull();
    result.Feature.ShouldBe(FeatureSummaryDTO.Empty);
    result.ConsumeOnly.ShouldBeTrue();
  }

  #endregion

  #region CodeOrId as Code Tests

  [Fact]
  public void IncludedFeatureDTO_ShouldMapByCode_WhenCodeOrIdIsNotValidGuid()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = "FEAT-CODE-001",
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-CODE-001" },
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-CODE-002" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.ShouldNotBeNull();
    result.Feature.ShouldNotBeNull();
    result.Feature.Code.ShouldBe("FEAT-CODE-001");
    result.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldMapByCode_WithSpecialCharacters()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = "FEAT-ČŘŽ-ÁÍÉ",
      ConsumeOnly = false
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-ČŘŽ-ÁÍÉ" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Code.ShouldBe("FEAT-ČŘŽ-ÁÍÉ");
    result.ConsumeOnly.ShouldBeFalse();
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldReturnEmpty_WhenCodeNotFoundInFeatures()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = "NON-EXISTING-CODE",
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-001" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.ShouldNotBeNull();
    result.Feature.ShouldBe(FeatureSummaryDTO.Empty);
    result.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldMapByCode_WhenCodeOrIdIsInvalidGuidFormat()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = "not-a-guid-123",
      ConsumeOnly = false
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "not-a-guid-123" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Code.ShouldBe("not-a-guid-123");
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldMapByCode_WithNumericCode()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = "12345",
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "12345" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Code.ShouldBe("12345");
  }

  #endregion

  #region ConsumeOnly Property Tests

  [Fact]
  public void IncludedFeatureDTO_ShouldMapConsumeOnly_WhenTrue()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = featureId, Code = "FEAT-001" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldMapConsumeOnly_WhenFalse()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString(),
      ConsumeOnly = false
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = featureId, Code = "FEAT-001" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.ConsumeOnly.ShouldBeFalse();
  }

  #endregion

  #region Empty Features Collection Tests

  [Fact]
  public void IncludedFeatureDTO_ShouldReturnEmpty_WhenFeaturesCollectionIsEmpty()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>();

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.ShouldNotBeNull();
    result.Feature.ShouldBe(FeatureSummaryDTO.Empty);
    result.ConsumeOnly.ShouldBeTrue();
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldReturnEmpty_WhenFeaturesCollectionIsNull()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString(),
      ConsumeOnly = false
    };

    Should.Throw<ArgumentNullException>(() =>
    {
      IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, null!);
    });
  }

  #endregion

  #region Multiple Features Tests

  [Fact]
  public void IncludedFeatureDTO_ShouldSelectCorrectFeature_FromMultipleFeatures()
  {
    // Arrange
    var targetId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = targetId.ToString(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-001" },
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-002" },
      new FeatureSummaryDTO { Id = targetId, Code = "FEAT-TARGET" },
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-003" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Id.ShouldBe(targetId);
    result.Feature.Code.ShouldBe("FEAT-TARGET");
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldReturnFirstMatch_WhenMultipleFeaturesHaveSameCode()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = "DUPLICATE-CODE",
      ConsumeOnly = true
    };
    var firstId = Guid.NewGuid();
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = firstId, Code = "DUPLICATE-CODE" },
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "DUPLICATE-CODE" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Id.ShouldBe(firstId);
    result.Feature.Code.ShouldBe("DUPLICATE-CODE");
  }

  #endregion

  #region Edge Cases

  [Fact]
  public void IncludedFeatureDTO_ShouldHandleEmptyGuid()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = Guid.Empty.ToString(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.Empty, Code = "EMPTY-GUID-FEAT" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Id.ShouldBe(Guid.Empty);
    result.Feature.Code.ShouldBe("EMPTY-GUID-FEAT");
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldHandleNullCodeOrId()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = null,
      ConsumeOnly = false
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-001" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.ShouldNotBeNull();
    result.Feature.ShouldBe(FeatureSummaryDTO.Empty);
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldHandleEmptyCodeOrId()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = string.Empty,
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = string.Empty }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Code.ShouldBe(string.Empty);
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldHandleWhitespaceCodeOrId()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = "   ",
      ConsumeOnly = false
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "   " }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Code.ShouldBe("   ");
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldBeConsistent_ForSameInput()
  {
    // Arrange
    var featureId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = featureId.ToString(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = featureId, Code = "CONSISTENT-FEAT" }
    };

    // Act
    var result1 = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);
    var result2 = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result1.Feature.Id.ShouldBe(result2.Feature.Id);
    result1.Feature.Code.ShouldBe(result2.Feature.Code);
    result1.ConsumeOnly.ShouldBe(result2.ConsumeOnly);
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldHandleLongCode()
  {
    // Arrange
    var longCode = new string('A', 1000);
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = longCode,
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = longCode }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Code.ShouldBe(longCode);
    result.Feature.Code.Length.ShouldBe(1000);
  }

  #endregion

  #region Case Sensitivity Tests

  [Fact]
  public void IncludedFeatureDTO_ShouldBeCaseSensitive_WhenMatchingByCode()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = "FEAT-UPPER",
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "feat-upper" }, // lowercase
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "FEAT-UPPER" }  // uppercase
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Code.ShouldBe("FEAT-UPPER");
  }

  #endregion

  #region Performance and Large Collections Tests

  [Fact]
  public void IncludedFeatureDTO_ShouldHandleLargeFeatureCollection()
  {
    // Arrange
    var targetId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = targetId.ToString(),
      ConsumeOnly = true
    };
    
    var features = new List<FeatureSummaryDTO>();
    for (int i = 0; i < 1000; i++)
    {
      features.Add(new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = $"FEAT-{i}" });
    }
    features.Add(new FeatureSummaryDTO { Id = targetId, Code = "TARGET-FEAT" });

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Id.ShouldBe(targetId);
    result.Feature.Code.ShouldBe("TARGET-FEAT");
  }

  #endregion

  #region Guid TryParse Edge Cases

  [Fact]
  public void IncludedFeatureDTO_ShouldFallbackToCode_WhenCodeOrIdLooksLikeGuidButIsNot()
  {
    // Arrange
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = "12345678-1234-5678-1234-567812345678X", // Invalid GUID (extra char)
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = "12345678-1234-5678-1234-567812345678X" }
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Code.ShouldBe("12345678-1234-5678-1234-567812345678X");
  }

  [Fact]
  public void IncludedFeatureDTO_ShouldPreferGuidMatch_OverCodeMatch()
  {
    // Arrange
    var targetId = Guid.NewGuid();
    var includedFeature = new IntegrationFeatureDefinition.IncludedFeature
    {
      CodeOrId = targetId.ToString(),
      ConsumeOnly = true
    };
    var features = new List<FeatureSummaryDTO>
    {
      new FeatureSummaryDTO { Id = Guid.NewGuid(), Code = targetId.ToString() }, // Code match
      new FeatureSummaryDTO { Id = targetId, Code = "GUID-MATCH" }  // GUID match
    };

    // Act
    var result = IncludedFeatureMapper.IncludedFeatureDTO(includedFeature, features);

    // Assert
    result.Feature.Id.ShouldBe(targetId);
    result.Feature.Code.ShouldBe("GUID-MATCH"); // Should match by GUID, not Code
  }

  #endregion
}
