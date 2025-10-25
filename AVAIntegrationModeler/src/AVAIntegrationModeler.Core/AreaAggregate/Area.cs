using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Core.AreaAggregate;
/// <summary>
/// Třida představující oblast v rámci systému. Oblast může sloužit k logickému seskupení různých entit, funkcí nebo komponent systému podle jejich účelu, domény nebo jiných kritérií.
/// </summary>
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
