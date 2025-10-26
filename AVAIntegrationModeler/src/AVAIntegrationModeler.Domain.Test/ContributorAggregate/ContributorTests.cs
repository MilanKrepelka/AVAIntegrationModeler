using AVAIntegrationModeler.Domain.ContributorAggregate;
using Shouldly;

namespace AVAIntegrationModeler.Domain.Test.ContributorAggregate;

/// <summary>
/// Unit testy pro agregát Contributor.
/// </summary>
public class ContributorTests
{
  [Fact]
  public void Constructor_ShouldSetName()
  {
    // Arrange
    var name = "John Doe";

    // Act
    var contributor = new Contributor(name);

    // Assert
    contributor.Name.ShouldBe(name);
    contributor.Status.ShouldBe(ContributorStatus.NotSet);
  }

  [Fact]
  public void Constructor_WithNullOrEmptyName_ShouldThrowException()
  {
    // Act & Assert
    Should.Throw<ArgumentException>(() => new Contributor(null!));
    Should.Throw<ArgumentException>(() => new Contributor(string.Empty));
  }

  [Fact]
  public void UpdateName_ShouldChangeName()
  {
    // Arrange
    var contributor = new Contributor("Original Name");
    var newName = "Updated Name";

    // Act
    contributor.UpdateName(newName);

    // Assert
    contributor.Name.ShouldBe(newName);
  }

  [Fact]
  public void UpdateName_WithNullOrEmpty_ShouldThrowException()
  {
    // Arrange
    var contributor = new Contributor("John Doe");

    // Act & Assert
    Should.Throw<ArgumentException>(() => contributor.UpdateName(null!));
    Should.Throw<ArgumentException>(() => contributor.UpdateName(string.Empty));
  }

  [Fact]
  public void SetPhoneNumber_ShouldUpdatePhoneNumber()
  {
    // Arrange
    var contributor = new Contributor("John Doe");
    var phoneNumber = "+420123456789";

    // Act
    contributor.SetPhoneNumber(phoneNumber);

    // Assert
    contributor.PhoneNumber.ShouldNotBeNull();
    contributor.PhoneNumber.Number.ShouldBe(phoneNumber);
  }

  [Fact]
  public void FluentAPI_ShouldChainMethodCalls()
  {
    // Act
    var contributor = new Contributor("John Doe")
      .UpdateName("Jane Doe")
      .SetPhoneNumber("+420987654321");

    // Assert
    contributor.Name.ShouldBe("Jane Doe");
    contributor.PhoneNumber.ShouldNotBeNull();
  }
}
