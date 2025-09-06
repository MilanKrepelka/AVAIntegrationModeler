using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.UseCases.Scenarios;

namespace AVAIntegrationModeler.UseCases;
/// <summary>
/// Obecné rozhraní pro mapování mezi doménovým objektem scénáře a jeho DTO.
/// </summary>
public interface IMapper<TDomainEntity, TDTO, TSelf>
    where TSelf : IMapper<TDomainEntity, TDTO, TSelf>
{

  /// <summary>
  /// Mapuje doménový objekt na jeho DTO reprezentaci.
  /// </summary>
  /// <param name="domainEntity"></param>
  /// <returns></returns>
  static abstract TDTO MapToDTO(TDomainEntity domainEntity);
  /// <summary>
  /// Mapuje DTO na doménový objekt.
  /// </summary>
  /// <param name="dto">DTO přiřazený k doménové entitě</param>
  /// <returns>Doménová entita</returns>
  static abstract TDomainEntity MapToEntity(TDTO dto);
}
