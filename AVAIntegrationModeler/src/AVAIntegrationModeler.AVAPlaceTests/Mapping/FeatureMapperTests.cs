using System;
using Xunit;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Ardalis.GuardClauses;
using ASOL.DataService.Contracts;

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
    [InlineData("ÚnicodéČódě")]
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
}
