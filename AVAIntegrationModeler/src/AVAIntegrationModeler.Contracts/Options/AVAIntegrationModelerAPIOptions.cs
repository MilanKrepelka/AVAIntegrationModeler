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

  /// <summary>
  /// Timeout (v sekundách) pro hromadný import datových modelů z AVAPlace.
  /// Operace může u velkého počtu modelů trvat výrazně déle než běžná volání API,
  /// proto má vlastní, delší timeout nezávislý na <see cref="TimeoutInSeconds"/>.
  /// </summary>
  public int BulkImportTimeoutInSeconds { get; set; } = 600;
}
