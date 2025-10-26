using AVAIntegrationModeler.Domain.ValueObjects;
using Shouldly;

namespace AVAIntegrationModeler.Domain.Test.ValueObjects;

/// <summary>
/// Unit testy pro ValueObject LocalizedValue.
/// </summary>
public class LocalizedValueTests
{
  [Fact]
  public void Equals_WithSameValues_ShouldReturnTrue()
  {
    // Arrange
    var value1 = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };
    var value2 = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };

    // Act & Assert
    value1.Equals(value2).ShouldBeTrue();
  }

  [Fact]
  public void Equals_WithDifferentCzechValue_ShouldReturnFalse()
  {
    // Arrange
    var value1 = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };
    var value2 = new LocalizedValue { CzechValue = "Nazdar", EnglishValue = "Hello" };

    // Act & Assert
    value1.Equals(value2).ShouldBeFalse();
  }

  [Fact]
  public void Equals_WithNull_ShouldReturnFalse()
  {
    // Arrange
    var value = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };

    // Act & Assert
    value.Equals(null).ShouldBeFalse();
  }

  [Fact]
  public void Equals_WithReferenceEquals_ShouldReturnTrue()
  {
    // Arrange
    var value = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };

    // Act & Assert
    value.Equals(value).ShouldBeTrue();
  }
}
