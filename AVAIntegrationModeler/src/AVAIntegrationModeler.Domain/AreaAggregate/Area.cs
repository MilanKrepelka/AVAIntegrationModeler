using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Domain.AreaAggregate;
/// <summary>
/// Třida představující oblast v rámci systému. Oblast může sloužit k logickému seskupení různých entit, funkcí nebo komponent systému podle jejich účelu, domény nebo jiných kritérií.
/// </summary>
public class Area : EntityBase<Guid>, IAggregateRoot
{
  public Area() { } // EF Core
  
  public Area(Guid id, string code)
  {
    Id = id;
    SetCode(code);
  }

  /// <summary>
  /// Kód oblasti.
  /// </summary>
  public string Code { get; private set; } = string.Empty;

  /// <summary>
  /// Název oblasti.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  public Area SetCode(string code)
  {
    Code = Guard.Against.NullOrEmpty(code, nameof(code));
    return this;
  }

  public Area SetName(string name)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    return this;
  }
}
