using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Infrastructure.ValidationServices;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.ValidationServices;

/// <summary>
/// Testy pro ContributorValidationService — Contributor nemá žádné cross-aggregate
/// pravidlo, takže obě metody musí vždy vrátit úspěch.
/// </summary>
public class ContributorValidationServiceTests
{
  private readonly ContributorValidationService _sut = new();

  [Fact]
  public async Task ValidateForCreate_AlwaysReturnsSuccess()
  {
    var contributor = new Contributor("Jan Novák");

    var result = await _sut.ValidateForCreate(Datasource.Database, contributor, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task Validate_AlwaysReturnsSuccess()
  {
    var contributor = new Contributor("Jan Novák");

    var result = await _sut.Validate(Datasource.Database, contributor, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }
}
