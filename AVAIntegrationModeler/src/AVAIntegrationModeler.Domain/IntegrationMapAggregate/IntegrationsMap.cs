using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Domain.IntegrationMapAggregate;
internal class IntegrationsMap : EntityBase<Guid>, IAggregateRoot
{
  /// <summary>
  /// Identifikátor oblasti
  /// </summary>
  public Guid AreaId { get; private set;  } = Guid.Empty;

  
}
