using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Scenarios.Mapping;
public class LocalizedValueMapper : IMapper<Core.ValueObjects.LocalizedValue , LocalizedValue, LocalizedValueMapper>
{
  private const string czechLocale = "cs-CZ";
  private const string englishLocale = "en-US";
  public static LocalizedValue MapToDTO(Core.ValueObjects.LocalizedValue domainEntity)
  {
    var result = new LocalizedValue();
    //LocalizedValue result = new LocalizedValue();
    ////Guard.Against.NullOrEmpty(domainEntity.Values, $"{nameof(LocalizedValueMapper)} - {nameof(domainEntity)} - {nameof(domainEntity.Values)}");
    //if (domainEntity.Values == null || domainEntity.Values.Count == 0)
    //{
    //  result.CzechValue = string.Empty;
    //  result.EnglishValue = string.Empty;
    //  return result;
    //}
    //result.CzechValue = domainEntity.Values.FirstOrDefault(item => item.Locale == czechLocale) == default ? string.Empty : domainEntity.Values.First(item => item.Locale == czechLocale).Value;
    //result.EnglishValue = domainEntity.Values.FirstOrDefault(item => item.Locale == englishLocale) == default ? string.Empty : domainEntity.Values.First(item => item.Locale == englishLocale).Value;
    return result;
  }

  public static Core.ValueObjects.LocalizedValue MapToEntity(LocalizedValue dto)
  {
    var result = new Core.ValueObjects.LocalizedValue();
    //LocalizedValue<string> result = new LocalizedValue<string>();
    //result.Values = new List<LocalizedValueItem<string>>();
    //if (!string.IsNullOrEmpty(dto.CzechValue))
    //{
    //  result.Values.Add(new LocalizedValueItem<string>() { Locale = czechLocale, Value = dto.CzechValue });
    //}

    //if (!string.IsNullOrEmpty(dto.EnglishValue))
    //{
    //  result.Values.Add(new LocalizedValueItem<string>() { Locale = englishLocale, Value = dto.EnglishValue });
    //}
    return result;
  }
}
