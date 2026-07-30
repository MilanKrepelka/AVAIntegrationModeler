using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;

namespace AVAIntegrationModeler.UseCases.DataModelRecords;

public static class DataModelRecordMapper
{
  public static DataModelRecordDTO MapToDTO(DataModelRecord entity)
  {
    Guard.Against.Null(entity, nameof(entity));
    return new DataModelRecordDTO
    {
      Id = entity.Id,
      ModelId = entity.ModelId,
      ExternalId = entity.ExternalId,
      Fields = entity.Fields.Select(f => new DataModelRecordFieldDTO
      {
        Key = f.Key,
        IsLocalized = f.IsLocalized,
        StringValue = f.StringValue,
        CzechValue = f.CzechValue,
        EnglishValue = f.EnglishValue
      }).ToList()
    };
  }

  public static DataModelRecord MapToEntity(DataModelRecordDTO dto)
  {
    Guard.Against.Null(dto, nameof(dto));
    var record = new DataModelRecord(
      dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
      dto.ModelId);

    record.SetExternalId(dto.ExternalId);

    foreach (var fieldDto in dto.Fields)
    {
      var field = new DataModelRecordField(Guid.NewGuid(), fieldDto.Key);
      if (fieldDto.IsLocalized)
        field.SetLocalizedValue(fieldDto.CzechValue, fieldDto.EnglishValue);
      else
        field.SetStringValue(fieldDto.StringValue);
      record.AddField(field);
    }

    return record;
  }
}
