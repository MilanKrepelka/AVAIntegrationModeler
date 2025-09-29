using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ASOL.DataService.Connector;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;


namespace AVAIntegrationModeler.AVAPlace.Mapping;
/// <summary>
/// Statická třída pro mapování mezi doménovým objektem scénáře (<see cref="Scenario"/>) a jeho datovým přenosovým objektem (<see cref="ScenarioDTO"/>).
/// Poskytuje metody pro převod mezi těmito dvěma reprezentacemi.
/// </summary>
public class ScenarioMapper : IMapper<IntegrationScenarioDefinition, ScenarioDTO, ScenarioMapper>
{
  public static ScenarioDTO MapToDTO(IntegrationScenarioDefinition domainEntity)
  {
    Guard.Against.Null(domainEntity, $"{nameof(ScenarioMapper)} - {nameof(domainEntity)}");
    Guard.Against.NullOrEmpty(domainEntity.Id, $"{nameof(ScenarioMapper)} - {nameof(domainEntity)} - {nameof(domainEntity.Id)} ");


    return new ScenarioDTO()
    {
      Code = domainEntity.Code,
      Id = Guid.Parse(domainEntity.Id),
      InputFeatureId = string.IsNullOrEmpty(domainEntity.InputFeatureCodeOrId) ? null : Guid.Parse(domainEntity.InputFeatureCodeOrId),
      OutputFeatureId = string.IsNullOrEmpty(domainEntity.OutputFeatureCodeOrId) ? null : Guid.Parse(domainEntity.OutputFeatureCodeOrId),
      Name = LocalizedValueMapper.MapToDTO(domainEntity.Name),
      Description = LocalizedValueMapper.MapToDTO(domainEntity.Description),
    };
  }

  public static IntegrationScenarioDefinition MapToEntity(ScenarioDTO dto)
  {
    throw new NotImplementedException();
  }
}
