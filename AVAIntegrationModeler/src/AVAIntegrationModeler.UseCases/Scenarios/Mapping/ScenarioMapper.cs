using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.ScenarioAggregate;

namespace AVAIntegrationModeler.UseCases.Scenarios.Mapping;

/// <summary>
/// Statická třída pro mapování mezi doménovým objektem scénáře (<see cref="Scenario"/>) a jeho datovým přenosovým objektem (<see cref="ScenarioDTO"/>).
/// </summary>
public static class ScenarioMapper
{
  /// <summary>
  /// Mapuje doménový objekt scénáře (<see cref="Scenario"/>) na jeho datový přenosový objekt (<see cref="ScenarioDTO"/>).
  /// </summary>
  /// <param name="scenario">Doménový objekt scénáře.</param>
  /// <returns><see cref="ScenarioDTO"/> nebo null, pokud je vstup null.</returns>
  public static ScenarioDTO MapToScenarioDTO(Scenario scenario)
  {
   Guard.Against.Null(scenario, nameof(scenario));

    ScenarioDTO result = new ScenarioDTO()
    {
      Id = scenario.Id,
      Code = scenario.Code,
      Name = LocalizedValueMapper.MapToDTO(scenario.Name),
      Description = LocalizedValueMapper.MapToDTO(scenario.Description),
      InputFeatureId = scenario.InputFeature,
      OutputFeatureId = scenario.OutputFeature
    };
    return result;
  }

  /// <summary>
  /// Převede datový přenosový objekt scénáře (<see cref="ScenarioDTO"/>) na doménový objekt scénáře (<see cref="Scenario"/>).
  /// </summary>
  /// <param name="dto">DTO scénáře, který má být převeden.</param>
  /// <returns>Doménový objekt scénáře.</returns>
  public static Scenario MapToEntity(ScenarioDTO dto)
  {
    Guard.Against.Null(dto, nameof(dto));
    
    var scenario = new Scenario(dto.Id)
        .SetCode(dto.Code)
        .SetName(LocalizedValueMapper.MapToEntity(dto.Name))
        .SetDescription(LocalizedValueMapper.MapToEntity(dto.Description));

    if (dto.InputFeatureId.HasValue)
      scenario.SetInputFeature(dto.InputFeatureId);
    if (dto.OutputFeatureId.HasValue)
      scenario.SetOutputFeature(dto.OutputFeatureId);

    return scenario;
  }
}
