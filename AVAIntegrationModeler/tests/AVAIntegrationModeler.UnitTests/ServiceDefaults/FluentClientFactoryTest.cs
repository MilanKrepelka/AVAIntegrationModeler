using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.ServiceDefaults;

namespace AVAIntegrationModeler.UnitTests.ServiceDefaults;
public class FluentClientFactoryTest
{
  [Fact()]
  public void Create_Test()
  {
    FluentClientFactory factory = new FluentClientFactory(new FluentClientOptions()
    {
      BaseUrl = "https://example.com/api"
    });

    var client = factory.CreateClient();

    client.ShouldNotBeNull();
    client.BaseClient.BaseAddress!.ToString().ShouldBe("https://example.com/api");
  }
}
