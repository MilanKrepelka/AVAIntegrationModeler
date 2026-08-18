using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.Infrastructure.ValidationServices;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.ValidationServices;

/// <summary>
/// Testy pro DataModelRecordValidationService — ExternalId nemá dnes vynucenou unikátnost,
/// takže obě metody musí vždy vrátit úspěch.
/// </summary>
public class DataModelRecordValidationServiceTests
{
  private readonly DataModelRecordValidationService _sut = new();

  [Fact]
  public async Task ValidateForCreate_AlwaysReturnsSuccess()
  {
    var record = new DataModelRecord(Guid.NewGuid(), Guid.NewGuid());

    var result = await _sut.ValidateForCreate(Datasource.Database, record, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task Validate_AlwaysReturnsSuccess()
  {
    var record = new DataModelRecord(Guid.NewGuid(), Guid.NewGuid());

    var result = await _sut.Validate(Datasource.Database, record, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }
}
