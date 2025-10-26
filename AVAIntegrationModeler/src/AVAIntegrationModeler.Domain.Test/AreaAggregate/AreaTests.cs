using AVAIntegrationModeler.Domain.AreaAggregate;
using Shouldly;

namespace AVAIntegrationModeler.Domain.Test.AreaAggregate;

/// <summary>
/// Unit testy pro agregát Area.
/// </summary>
public class AreaTests
{
  [Fact]
  public void Area_ShouldHaveEmptyCodeAndNameByDefault()
  {
    // Act
    var area = new Area();

    // Assert
    area.Code.ShouldBe(string.Empty);
    area.Name.ShouldBe(string.Empty);
  }

  [Fact]
  public void Area_ShouldAllowSettingCodeAndName()
  {
    // Arrange
    var area = new Area();

    // Act
    area.SetCode("AREA-001");
    area.SetName("Test Area");

    // Assert
    area.Code.ShouldBe("AREA-001");
    area.Name.ShouldBe("Test Area");
  }
}
