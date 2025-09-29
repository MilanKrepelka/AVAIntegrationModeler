using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlaceTests.Fixtures;
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
    return Task.CompletedTask;

    /*
    ServiceRuntimeTenantContext.ExecuteInContextAsync(_fixture.GetServiceProvider(_testOutputHelper)).
    var result = await dataserviceClient.GetDataAgentsAsync(new ASOL.Core.Paging.Contracts.Filters.PagingFilter() { }, "ASOLEU-DEV-fd9ad6b9-2f29-4c7a-9a3a-c7469e19b1ff-AVAPlaceModeler-ACw4KkqwqV8E9MFdnIumTmzv9R5IUPsO", CancellationToken.None);
    */

    //var serviceProvirer = _fixture.GetServiceProvider(_testOutputHelper);

    //using (var scope = serviceProvirer.CreateScope())
    //{
    //  var scopedProvider = scope.ServiceProvider;
    //  //get connector instance via dependency injection and use impersonated context
    //  await ServiceRuntimeTenantContext.ExecuteInContextAsync<ICustomDataServiceClient>(scopedProvider, "ASOLEU-DEV-fd9ad6b9-2f29-4c7a-9a3a-c7469e19b1ff", async connector =>
    //  {
    //    return Task.CompletedTask;
    //    //var cc = await connector.IntegrationScenarios.GetScenarioAsyncGetDataSourceAsync(Guid.Parse("efaf9059-757b-4a3f-91eb-188cd11d9e8d"), false, CancellationToken.None);
    //    //one-time initialization / patch supported features of integration-agent
    //  });
    //}
  }
}
