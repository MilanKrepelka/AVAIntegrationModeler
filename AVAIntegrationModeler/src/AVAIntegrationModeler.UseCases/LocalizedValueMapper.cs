using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases;
public static class LocalizedValueMapper
{
  private const string czechLocale = "cs-CZ";
  private const string englishLocale = "en-US";
  public static LocalizedValue MapToDTO(Domain.ValueObjects.LocalizedValue domainEntity)
  {
    if (domainEntity is null)
    {
      return new LocalizedValue()
      {
        CzechValue = string.Empty,
        EnglishValue = string.Empty
      };
    }
    return new LocalizedValue()
    {
      CzechValue = domainEntity.CzechValue ?? string.Empty,
      EnglishValue = domainEntity.EnglishValue ?? string.Empty
    };
  }    

  public static Domain.ValueObjects.LocalizedValue MapToEntity(LocalizedValue dto)
  {
    if (dto is null)
    {
      return new Domain.ValueObjects.LocalizedValue()
      {
        CzechValue = string.Empty,
        EnglishValue = string.Empty
      };
    }
    return new Domain.ValueObjects.LocalizedValue()
    {
      CzechValue = dto.CzechValue ?? string.Empty,
      EnglishValue = dto.EnglishValue ?? string.Empty
    };
  }
}
