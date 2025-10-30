using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts.DTO;
public record IncludedDataModelDTO
{
  public DataModelSummaryDTO DataModel { get; init; } = new();
  public bool ReadOnly { get; init; }
}
