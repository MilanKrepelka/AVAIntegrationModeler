using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Core.ValueObjects;

namespace AVAIntegrationModeler.UnitTests.Core.ValueObjects;

public class LocalizedValueTest
{
  [Fact]
  public void Equals_ReturnsTrue_ForSameValues()
  {
    var value1 = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };
    var value2 = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };

    Assert.True(value1.Equals(value2));
  }

  [Fact]
  public void Equals_ReturnsFalse_ForDifferentCzechValue()
  {
    var value1 = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };
    var value2 = new LocalizedValue { CzechValue = "Nazdar", EnglishValue = "Hello" };

    Assert.False(value1.Equals(value2));
  }

  [Fact]
  public void Equals_ReturnsFalse_ForDifferentEnglishValue()
  {
    var value1 = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };
    var value2 = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hi" };

    Assert.False(value1.Equals(value2));
  }

  [Fact]
  public void Equals_ReturnsFalse_ForNull()
  {
    var value = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };

    Assert.False(value.Equals(null));
  }

  [Fact]
  public void Equals_ReturnsTrue_ForReferenceEquals()
  {
    var value = new LocalizedValue { CzechValue = "Ahoj", EnglishValue = "Hello" };

    Assert.True(value.Equals(value));
  }

  [Fact]
  public void Equals_ReturnsTrue_ForNonSetEnglishValue()
  {
    var value1 = new LocalizedValue { CzechValue = "Ahoj" };
    var value2 = new LocalizedValue { CzechValue = "Ahoj" };

    Assert.True(value1.Equals(value2));
  }

  [Fact]
  public void Equals_ReturnsTrue_ForNonSetCzechValue()
  {
    var value1 = new LocalizedValue { EnglishValue = "Hi" };
    var value2 = new LocalizedValue { EnglishValue = "Hi" };

    Assert.True(value1.Equals(value2));
  }
}
