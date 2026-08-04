namespace AVAIntegrationModeler.Domain.DataModelRecordAggregate;

public class DataModelRecord : EntityBase<Guid>, IAggregateRoot
{
  private DataModelRecord() { }

  public DataModelRecord(Guid id, Guid modelId)
  {
    Id = id;
    SetModelId(modelId);
  }

  public Guid ModelId { get; private set; }

  public string ExternalId { get; private set; } = string.Empty;

  private readonly List<DataModelRecordField> _fields = new();

  public IReadOnlyCollection<DataModelRecordField> Fields => _fields.AsReadOnly();

  public DataModelRecord SetModelId(Guid modelId)
  {
    ModelId = Guard.Against.Default(modelId, nameof(modelId));
    return this;
  }

  public DataModelRecord SetExternalId(string externalId)
  {
    ExternalId = externalId ?? string.Empty;
    return this;
  }

  public DataModelRecord AddField(DataModelRecordField field)
  {
    Guard.Against.Null(field, nameof(field));
    if (_fields.Any(f => f.Key.Equals(field.Key, StringComparison.OrdinalIgnoreCase)))
      throw new InvalidOperationException($"Field with key '{field.Key}' already exists in this record.");
    _fields.Add(field);
    return this;
  }

  public DataModelRecord RemoveField(string key)
  {
    Guard.Against.NullOrEmpty(key, nameof(key));
    var field = _fields.FirstOrDefault(f => f.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
    if (field != null) _fields.Remove(field);
    return this;
  }

  public DataModelRecordField? GetField(string key)
  {
    Guard.Against.NullOrEmpty(key, nameof(key));
    return _fields.FirstOrDefault(f => f.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
  }
}
