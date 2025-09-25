using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASOL.Core.Identity.Extensions;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlace.API.Connectors;
using AVAIntegrationModeler.AVAPlace.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AVAIntegrationModeler.AVAPlaceTests.Fixtures;
public class AVAPlaceDemoFixture : TestBedFixture
{
  protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
  {
    services.AddOptions();
    services.AddRuntimeContexts();
    services.AddRuntimeContextScope();
    services.AddMultitenancySetup();
    services.AddApiConnectors(configuration!);
    //services.AddDataServiceClientExtended();
  }

  protected override ValueTask DisposeAsyncCore()
  {
    return new ValueTask(Task.CompletedTask);
  }

  protected override IEnumerable<TestAppSettings> GetTestAppSettings()
  {
    return new List<TestAppSettings>(){
    new TestAppSettings()
    {
      Filename = "appsettings.demo.json",
      IsOptional = false,
    }
    };
  }
}
