using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.ViewModels.List;

namespace AVAIntegrationModeler.Web.Mapping;

/// <summary>
/// Implementuje mapování mezi datovým přenosovým objektem datového modelu (<see cref="DataModelDTO"/>) a jeho zobrazeními ve webovém rozhraní.
/// </summary>
public static class DataModelMapper
{
  /// <summary>
  /// Mapuje <see cref="DataModelDTO"/> na <see cref="DataModelListViewModel"/>.
  /// </summary>
  /// <param name="dto">DTO datového modelu k namapování.</param>
  /// <returns>ViewModel datového modelu.</returns>
  public static DataModelListViewModel MapToViewModel(DataModelDTO dto, IEnumerable<DataModelDTO> dataModels)
  {
    Guard.Against.Null(dto, $"{nameof(DataModelMapper)} - {nameof(dto)}");

    return new DataModelListViewModel
    {
      Id = dto.Id,
      Code = dto.Code,
      Name = StringToLocalizedValue(dto.Name),
      Description = StringToLocalizedValue(dto.Description),
      Notes = dto.Notes,
      IsAggregateRoot = dto.IsAggregateRoot,
      AreaId = dto.AreaId,
      Fields = dto.Fields.Select(field => MapFieldToViewModel(field, dataModels)).ToList()
    };
  }

  /// <summary>
  /// Mapuje <see cref="DataModelFieldDTO"/> na <see cref="DataModelFieldListViewModel"/>.
  /// </summary>
  /// <param name="dto">DTO pole datového modelu k namapování.</param>
  /// <returns>ViewModel pole datového modelu.</returns>
  public static DataModelFieldListViewModel MapFieldToViewModel(DataModelFieldDTO dto, IEnumerable<DataModelDTO> dataModels)
  {
    Guard.Against.Null(dto, $"{nameof(DataModelMapper)} - {nameof(dto)}");

    return new DataModelFieldListViewModel
    {
      Id = dto.Id,
      Name = dto.Name,
      Label = dto.Label,
      Description = dto.Description,
      IsPublishedForLookup = dto.IsPublishedForLookup,
      IsCollection = dto.IsCollection,
      IsLocalized = dto.IsLocalized,
      IsNullable = dto.IsNullable,
      FieldType = dto.FieldType,
      ReferencedEntityTypeIds = dto.ReferencedEntityTypeIds,
      ReferencedModels = dataModels
        .Where(dm => dto.ReferencedEntityTypeIds.Contains(dm.Id))
        .ToList()
    };
  }

  /// <summary>
  /// Převede string na LocalizedValue (používá stejný text pro češtinu i angličtinu).
  /// </summary>
  /// <param name="value">Textová hodnota k převedení.</param>
  /// <returns>LocalizedValue s hodnotou v obou jazycích.</returns>
  private static LocalizedValue StringToLocalizedValue(string value)
  {
    return string.IsNullOrEmpty(value) 
      ? LocalizedValue.Empty 
      : new LocalizedValue { CzechValue = value, EnglishValue = value };
  }
}
