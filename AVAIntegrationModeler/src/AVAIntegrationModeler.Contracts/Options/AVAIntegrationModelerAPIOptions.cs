using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts.Options;
public class AVAIntegrationModelerAPIOptions
{
  public string BaseUrl { get; set; } = string.Empty;
  public int TimeoutInSeconds { get; set; } = 10;
}
