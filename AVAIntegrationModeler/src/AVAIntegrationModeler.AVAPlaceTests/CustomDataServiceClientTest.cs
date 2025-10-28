using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlaceTests.Fixtures;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Testing.Platform.Services;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AVAIntegrationModeler.AVAPlaceTests;

public class CustomDataServiceClientTest : TestBed<Fixtures.AVAPlaceDemoFixture>
{
  public CustomDataServiceClientTest(ITestOutputHelper testOutputHelper, AVAPlaceDemoFixture fixture) : base(testOutputHelper, fixture)
  {
  }

  [Fact]
  public Task GetScenariosTest()
  {
    

    /*
    ServiceRuntimeTenantContext.ExecuteInContextAsync(_fixture.GetServiceProvider(_testOutputHelper)).
    var result = await dataserviceClient.GetDataAgentsAsync(new ASOL.Core.Paging.Contracts.Filters.PagingFilter() { }, "ASOLEU-DEV-fd9ad6b9-2f29-4c7a-9a3a-c7469e19b1ff-AVAPlaceModeler-ACw4KkqwqV8E9MFdnIumTmzv9R5IUPsO", CancellationToken.None);
    */

    //var serviceProvirer = _fixture.GetServiceProvider(_testOutputHelper);

    //using (var scope = serviceProvirer.CreateScope())
    //{
    //  var scopedProvider = scope.ServiceProvider;
    //  //get connector instance via dependency injection and use impersonated context
      
    //  _ = await ServiceRuntimeTenantContext.ExecuteInContextAsync<ICustomDataServiceClient, IEnumerable<ScenarioDTO>>(scopedProvider, "ASOLEU-DEV-fd9ad6b9-2f29-4c7a-9a3a-c7469e19b1ff", async connector =>
    //  {
    //    var result = await connector.GetScenariosAsync(CancellationToken.None);
    //    Assert.NotNull(result);
    //    Assert.NotEmpty(result);
    //    return result;
    //  });
    //}
    return Task.CompletedTask;
  }
}
