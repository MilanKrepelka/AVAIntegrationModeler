using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Core.ScenarioAggregate;

namespace AVAIntegrationModeler.UseCases.Scenarios.Mapping;
/// <summary>
/// Statická třída pro mapování mezi doménovým objektem scénáře (<see cref="Scenario"/>) a jeho datovým přenosovým objektem (<see cref="ScenarioDTO"/>).
/// Poskytuje metody pro převod mezi těmito dvěma reprezentacemi.
/// </summary>
public class ScenarioMapper : IMapper<Scenario, ScenarioDTO, ScenarioMapper>
{
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
            .SetId(dto.Id)
            .SetName(LocalizedValueMapper.MapToEntity(dto.Name))
            .SetDescription(LocalizedValueMapper.MapToEntity(dto.Description));

        if (dto.InputFeatureId.HasValue)
            scenario.SetInputFeature(dto.InputFeatureId);
        if (dto.OutputFeatureId.HasValue)
            scenario.SetOutputFeature(dto.OutputFeatureId);

        return scenario;
    }

    /// <summary>
    /// Převede doménový objekt scénáře (<see cref="Scenario"/>) na jeho datový přenosový objekt (<see cref="ScenarioDTO"/>).
    /// </summary>
    /// <param name="scenario">Doménový objekt scénáře, který má být převeden.</param>
    /// <returns>DTO scénáře.</returns>
    public static ScenarioDTO MapToDTO(Scenario scenario)
    {
        Guard.Against.Null(scenario, $"{nameof(ScenarioMapper)} - {nameof(scenario)}");
        return new ScenarioDTO
        {
            Id = scenario.Id,
            Code = scenario.Code,
            Name = scenario.Name,
            Description = scenario.Description,

            InputFeatureId = scenario.InputFeature,
            OutputFeatureId = scenario.OutputFeature
        };
    }
}
