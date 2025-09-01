using AVAIntegrationModeler.Core.ScenarioAggregate;
using AVAIntegrationModeler.Core.ContributorAggregate;

namespace AVAIntegrationModeler.IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
  [Fact]
  public async Task AddsContributorAndSetsId()
  {
    var testContributorName = "testContributor";
    var testContributorStatus = ContributorStatus.NotSet;
    var repository = GetRepository();
    var Contributor = new Contributor(testContributorName);

    await repository.AddAsync(Contributor);

    var newContributor = (await repository.ListAsync())
                    .FirstOrDefault();

    newContributor.ShouldNotBeNull();
    testContributorName.ShouldBe(newContributor.Name);
    testContributorStatus.ShouldBe(newContributor.Status);
    newContributor.Id.ShouldBeGreaterThan(0);
  }

  [Fact]
  public async Task AddsScenarioAndSetsId()
  {
    Guid testScenarioId = Guid.NewGuid();
    var testScenarioName = "testScenario";
    var testScenarioCode = "testScenarioCode";
    var repository = GetScenarioRepository();
    var Scenario = new Scenario(testScenarioId);

    await repository.AddAsync(Scenario);

    var newScenario = (await repository.ListAsync())
                    .FirstOrDefault();

    newScenario.ShouldNotBeNull();
    testScenarioName.ShouldBe(newScenario.Name.CzechValue);
    newScenario.Code.ShouldBe(testScenarioCode);
    newScenario.Id.ShouldBeGreaterThan(testScenarioId);
  }
}
