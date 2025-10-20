using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Core.ScenarioAggregate;
/// <summary>
/// Integrační feature v agregátu scénáře.
/// </summary>
public class Feature(Guid? id)
{
  /// <summary>
  /// Identifikátor integrační feature.
  /// </summary>
  public Guid? Id { get; internal set; } = id;

}
