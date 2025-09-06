using AVAIntegrationModeler.Core.ValueObjects;
using AVAIntegrationModeler.UseCases.Scenarios;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Represents a record of a scenario, inheriting from <see cref="ScenarioDTO"/>.
/// </summary>
/// <remarks>This record is used to encapsulate data related to a specific scenario.  It extends the functionality
/// of <see cref="ScenarioDTO"/> and is immutable by design.</remarks>
public record ScenarioRecord : ScenarioDTO
{
  public ScenarioRecord() { }
  public ScenarioRecord(ScenarioDTO dto)
      : base()
  {
    Id = dto.Id;
    Code = dto.Code;
    Name = dto.Name;
    Description = dto.Description;
    InputFeatureId = dto.InputFeatureId;
    OutputFeatureId = dto.OutputFeatureId;
  }
}

