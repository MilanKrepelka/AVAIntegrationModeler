using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts.DTO;
public record IncludedFeatureDTO
{
  public IncludedFeatureDTO()
  {
    
  }
  public FeatureSummaryDTO Feature { get; init; } = new();
  public bool ConsumeOnly { get; init; }
}
