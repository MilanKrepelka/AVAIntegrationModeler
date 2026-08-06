namespace AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

public class DataModelRecordListViewModel
{
  public static DataModelRecordListViewModel Empty => new();

  public Guid Id { get; set; }
  public Guid ModelId { get; set; }
  public string ModelName { get; set; } = string.Empty;
  public string ExternalId { get; set; } = string.Empty;
  public List<DataModelRecordFieldListViewModel> Fields { get; set; } = new();

  public string FieldsSummary => string.Join(", ", Fields.Take(3).Select(f =>
    f.IsLocalized ? $"{f.Key}: {f.CzechValue}" : $"{f.Key}: {f.StringValue}"));
}

public class DataModelRecordFieldListViewModel
{
  public string Key { get; set; } = string.Empty;
  public bool IsLocalized { get; set; }
  public string? StringValue { get; set; }
  public string? CzechValue { get; set; }
  public string? EnglishValue { get; set; }
}
