using AVAIntegrationModeler.Core.ContributorAggregate;
using AVAIntegrationModeler.Core.ScenarioAggregate;
using Shouldly;

namespace AVAIntegrationModeler.IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
  [Fact]
  public async Task AddsContributorAndSetsId()
  {
    var testContributorName = "testContributor";
    var testContributorStatus = ContributorStatus.NotSet;
    var repository = GetRepository();
    var contributor = new Contributor(testContributorName);

    await repository.AddAsync(contributor);

    var newContributor = (await repository.ListAsync()).Where(item=>item.Id == contributor.Id).FirstOrDefault();


    newContributor.ShouldNotBeNull();
    testContributorName.ShouldBe(newContributor.Name);
    testContributorStatus.ShouldBe(newContributor.Status);
    newContributor.Id.ShouldBeGreaterThan(0);
  }

  [Fact]
  public async Task AddsScenarioAndSetsValues()
  {
    Guid testScenarioId = Guid.NewGuid();
    var testScenarioName = new Core.ValueObjects.LocalizedValue()
    {
      CzechValue = "testovací scénáč",
      EnglishValue = "test Scenario"
    };
    var testScenarioCode = "testScenarioCode";
    var repository = GetScenarioRepository();
    var Scenario = new Scenario(testScenarioId);


    Scenario.SetName(new Core.ValueObjects.LocalizedValue() { 
      CzechValue = "testovací scénáč",
      EnglishValue = "test Scenario"
    })
            .SetCode(testScenarioCode)
            .SetDescription(new AVAIntegrationModeler.Core.ValueObjects.LocalizedValue { CzechValue = "Popis scénáře" })
            .SetInputFeature(null)
            .SetOutputFeature(null);
    await repository.AddAsync(Scenario);

    var newScenario = (await repository.ListAsync())
                    .Where(item=> item.Id == Scenario.Id).FirstOrDefault();

    newScenario.ShouldNotBeNull();
    newScenario.ShouldNotBeNull();
    newScenario.Name.ShouldNotBeNull();
    newScenario.Name.CzechValue.ShouldBe(testScenarioName.CzechValue);
    newScenario.Name.EnglishValue.ShouldBe(testScenarioName.EnglishValue);
    newScenario.Code.ShouldBe(testScenarioCode);
    newScenario.Id.ShouldBe(testScenarioId);
    newScenario.Description.ShouldNotBeNull();
    newScenario.Description.CzechValue.ShouldBe("Popis scénáře");
    newScenario.InputFeature.ShouldBeNull();
    newScenario.OutputFeature.ShouldBeNull();
  }
}
