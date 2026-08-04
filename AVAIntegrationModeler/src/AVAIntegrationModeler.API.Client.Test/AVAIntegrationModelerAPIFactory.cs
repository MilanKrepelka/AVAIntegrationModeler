using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace AVAIntegrationModeler.API.Client.Test;
public class AVAIntegrationModelerAPIFactory : WebApplicationFactory<AVAIntegrationModeler.API.Program>
{
  protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
  {

    builder.ConfigureServices((context, services) =>
    {
      services.AddAVAIntegrationModelerApiClient(context.Configuration);
    });
  }
}
