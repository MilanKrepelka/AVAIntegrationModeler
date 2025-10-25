using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Core.AreaAggregate;
public class Area : EntityBase<Guid>, IAggregateRoot
{
  /// <summary>
  /// Kód oblasti.
  /// </summary>
  public string Code { get; set; } = string.Empty;

  /// <summary>
  /// Název oblasti.
  /// </summary>
  public string Name { get; set; } = string.Empty;
}
