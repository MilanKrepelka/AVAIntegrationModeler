using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ASOL.Core.Localization;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.AVAPlace.Mapping;
public class LocalizedValueMapper : IMapper<LocalizedValue<string>, AVAIntegrationModeler.Contracts.DTO.LocalizedValue, LocalizedValueMapper>
{
  private const string czechLocale = "cs-CZ";
  private const string englishLocale = "en-US";
  
  /// <inheritdoc/>
  public static LocalizedValue MapToDTO(LocalizedValue<string> domainEntity)
  {
    if (domainEntity == default)
    {
      return LocalizedValue.Empty;
    }
    LocalizedValue result = new LocalizedValue();
    //Guard.Against.NullOrEmpty(domainEntity.Values, $"{nameof(LocalizedValueMapper)} - {nameof(domainEntity)} - {nameof(domainEntity.Values)}");
    if (domainEntity.Values == null || domainEntity.Values.Count == 0)  // ← Tady dochází k NullReferenceException
    {
      result.CzechValue = string.Empty;
      result.EnglishValue = string.Empty;
      return result;
    }
    result.CzechValue = domainEntity.Values.FirstOrDefault(item => item.Locale == czechLocale) == default ? string.Empty : domainEntity.Values.First(item => item.Locale == czechLocale).Value;
    result.EnglishValue = domainEntity.Values.FirstOrDefault(item => item.Locale == englishLocale) == default ? string.Empty : domainEntity.Values.First(item => item.Locale == englishLocale).Value;
    return result;
  }

  /// <inheritdoc/>
  public static LocalizedValue<string> MapToEntity(LocalizedValue dto)
  {
    LocalizedValue<string> result = new LocalizedValue<string>();
    result.Values = new List<LocalizedValueItem<string>>();
    if (!string.IsNullOrEmpty(dto.CzechValue))
    {
      result.Values.Add(new LocalizedValueItem<string>() { Locale = czechLocale, Value = dto.CzechValue });
    }

    if (!string.IsNullOrEmpty(dto.EnglishValue))
    {
      result.Values.Add(new LocalizedValueItem<string>() { Locale = englishLocale, Value = dto.EnglishValue });
    }
    return result;
  }
}
