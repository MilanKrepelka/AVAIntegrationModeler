using AVAIntegrationModeler.Core.ContributorAggregate;

namespace AVAIntegrationModeler.IntegrationTests.Data;

public class EfRepositoryDelete : BaseEfRepoTestFixture
{
  [Fact]
  public async Task DeletesItemAfterAddingIt()
  {
    // add a Contributor
    var repository = GetRepository();
    var initialName = Guid.NewGuid().ToString();
    var Contributor = new Contributor(initialName);
    await repository.AddAsync(Contributor, CancellationToken.None);

    // delete the item
    await repository.DeleteAsync(Contributor, CancellationToken.None);

    // verify it's no longer there
    (await repository.ListAsync(CancellationToken.None)).ShouldNotContain(Contributor => Contributor.Name == initialName);
  }
}
