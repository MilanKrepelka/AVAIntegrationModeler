namespace AVAIntegrationModeler.Domain.DataModelRecordAggregate;

public class DataModelRecordField : EntityBase<Guid>
{
  private DataModelRecordField() { }

  public DataModelRecordField(Guid id, string key)
  {
    Id = id;
    SetKey(key);
  }

  public string Key { get; private set; } = string.Empty;

  public bool IsLocalized { get; private set; }

  public string? StringValue { get; private set; }

  public string? CzechValue { get; private set; }

  public string? EnglishValue { get; private set; }

  public DataModelRecordField SetKey(string key)
  {
    Key = Guard.Against.NullOrEmpty(key, nameof(key));
    return this;
  }

  public DataModelRecordField SetStringValue(string? value)
  {
    IsLocalized = false;
    StringValue = value;
    CzechValue = null;
    EnglishValue = null;
    return this;
  }

  public DataModelRecordField SetLocalizedValue(string? czech, string? english)
  {
    IsLocalized = true;
    StringValue = null;
    CzechValue = czech;
    EnglishValue = english;
    return this;
  }
}
