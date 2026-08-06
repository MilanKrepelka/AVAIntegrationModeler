using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Mapping;

public static class DataModelRecordMapper
{
  /// <summary>
  /// Namapuje DTO záznamu na view model pro zobrazení v přehledu, včetně názvu DataModelu,
  /// ke kterému záznam patří.
  /// </summary>
  public static DataModelRecordListViewModel MapToViewModel(DataModelRecordDTO dto, IEnumerable<DataModelDTO> dataModels)
  {
    var model = dataModels.FirstOrDefault(m => m.Id == dto.ModelId);

    return new DataModelRecordListViewModel
    {
      Id = dto.Id,
      ModelId = dto.ModelId,
      ModelName = model?.Name ?? string.Empty,
      ExternalId = dto.ExternalId,
      Fields = dto.Fields.Select(f => new DataModelRecordFieldListViewModel
      {
        Key = f.Key,
        IsLocalized = f.IsLocalized,
        StringValue = f.StringValue,
        CzechValue = f.CzechValue,
        EnglishValue = f.EnglishValue
      }).ToList()
    };
  }
}
