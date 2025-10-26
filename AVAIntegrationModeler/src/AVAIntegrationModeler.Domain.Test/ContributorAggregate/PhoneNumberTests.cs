using AVAIntegrationModeler.Domain.ContributorAggregate;
using Shouldly;

namespace AVAIntegrationModeler.Domain.Test.ContributorAggregate;

/// <summary>
/// Unit testy pro ValueObject PhoneNumber.
/// </summary>
public class PhoneNumberTests
{
  [Fact]
  public void Constructor_ShouldSetAllProperties()
  {
    // Arrange
    var countryCode = "+420";
    var number = "123456789";
    var extension = "123";

    // Act
    var phoneNumber = new PhoneNumber(countryCode, number, extension);

    // Assert
    phoneNumber.CountryCode.ShouldBe(countryCode);
    phoneNumber.Number.ShouldBe(number);
    phoneNumber.Extension.ShouldBe(extension);
  }

  [Fact]
  public void Equals_WithSameValues_ShouldReturnTrue()
  {
    // Arrange
    var phone1 = new PhoneNumber("+420", "123456789", "123");
    var phone2 = new PhoneNumber("+420", "123456789", "123");

    // Act & Assert
    phone1.Equals(phone2).ShouldBeTrue();
  }

  [Fact]
  public void Equals_WithDifferentNumbers_ShouldReturnFalse()
  {
    // Arrange
    var phone1 = new PhoneNumber("+420", "123456789", null);
    var phone2 = new PhoneNumber("+420", "987654321", null);

    // Act & Assert
    phone1.Equals(phone2).ShouldBeFalse();
  }

  [Fact]
  public void Equals_WithNullExtension_ShouldWorkCorrectly()
  {
    // Arrange
    var phone1 = new PhoneNumber("+420", "123456789", null);
    var phone2 = new PhoneNumber("+420", "123456789", null);

    // Act & Assert
    phone1.Equals(phone2).ShouldBeTrue();
  }
}
