using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;


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
  /// <param name="dataModels">Seznam všech datových modelů pro mapování referencí.</param>
  /// <returns>ViewModel datového modelu.</returns>
  public static DataModelListViewModel MapToViewModel(DataModelDTO dto, IEnumerable<DataModelDTO> dataModels)
  {
    Guard.Against.Null(dto, $"{nameof(DataModelMapper)} - {nameof(dto)}");

    return new DataModelListViewModel
    {
      Id = dto.Id,
      Code = dto.Code,
      Name = dto.Name ?? string.Empty,
      Description = dto.Description ?? string.Empty,
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
  /// <param name="dataModels">Seznam všech datových modelů pro mapování referencí.</param>
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
}
