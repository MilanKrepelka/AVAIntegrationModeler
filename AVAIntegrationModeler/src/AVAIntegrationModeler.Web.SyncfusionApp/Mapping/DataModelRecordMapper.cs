using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Mapping;

public static class DataModelRecordMapper
{
  public static DataModelRecordListViewModel MapToViewModel(DataModelRecordDTO dto)
  {
    return new DataModelRecordListViewModel
    {
      Id = dto.Id,
      ModelId = dto.ModelId,
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
