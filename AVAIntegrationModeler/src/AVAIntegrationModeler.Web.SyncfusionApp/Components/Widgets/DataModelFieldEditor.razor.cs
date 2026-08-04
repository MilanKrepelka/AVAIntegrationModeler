using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Widgets;

public class DataModelFieldEditModel
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Name { get; set; } = string.Empty;
  public string Label { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DataModelFieldType FieldType { get; set; } = DataModelFieldType.Text;
  public bool IsPublishedForLookup { get; set; }
  public bool IsCollection { get; set; }
  public bool IsLocalized { get; set; }
  public bool IsNullable { get; set; }
  public List<Guid> ReferencedEntityTypeIds { get; set; } = [];
}

public partial class DataModelFieldEditor : ComponentBase
{
  [Parameter] public List<DataModelFieldEditModel> Fields { get; set; } = [];
  [Parameter] public List<DataModelSummaryDTO> AvailableModels { get; set; } = [];

  private SfGrid<DataModelFieldEditModel>? FieldGrid;
  private List<DataModelFieldEditModel>? _previousFields;

  private DataModelFieldEditModel? _editingField;
  private bool _isNew;
  private bool _isDialogOpen;
  private string _nameError = string.Empty;

  private static readonly List<FieldTypeItem> FieldTypeOptions =
    Enum.GetValues<DataModelFieldType>()
        .Select(ft => new FieldTypeItem { Value = ft, Text = GetFieldTypeLabel(ft) })
        .ToList();

  public record FieldTypeItem { public DataModelFieldType Value { get; init; } public string Text { get; init; } = ""; }

  public static string GetFieldTypeLabel(DataModelFieldType ft) => ft switch
  {
    DataModelFieldType.Text => "Text",
    DataModelFieldType.MultilineText => "Víceřádkový text",
    DataModelFieldType.TwoOptions => "Dvě možnosti (Ano/Ne)",
    DataModelFieldType.WholeNumber => "Celé číslo",
    DataModelFieldType.DecimalNumber => "Desetinné číslo",
    DataModelFieldType.UniqueIdentifier => "GUID",
    DataModelFieldType.UtcDateTime => "Datum a čas (UTC)",
    DataModelFieldType.LookupEntity => "Odkaz na entitu",
    DataModelFieldType.NestedEntity => "Vnořená entita",
    DataModelFieldType.Date => "Datum",
    DataModelFieldType.FileReference => "Odkaz na soubor",
    DataModelFieldType.CurrencyNumber => "Měna",
    DataModelFieldType.SingleSelectOptionSet => "Výběr jedné možnosti",
    DataModelFieldType.MultiSelectOptionSet => "Výběr více možností",
    _ => ft.ToString()
  };

  private bool NeedsReferences =>
    _editingField?.FieldType is DataModelFieldType.LookupEntity or DataModelFieldType.NestedEntity;

  protected override void OnParametersSet()
  {
    _previousFields = Fields;
  }

  public void OpenAdd()
  {
    _editingField = new DataModelFieldEditModel();
    _isNew = true;
    _nameError = string.Empty;
    _isDialogOpen = true;
  }

  public void OpenEdit(DataModelFieldEditModel field)
  {
    _editingField = new DataModelFieldEditModel
    {
      Id = field.Id,
      Name = field.Name,
      Label = field.Label,
      Description = field.Description,
      FieldType = field.FieldType,
      IsPublishedForLookup = field.IsPublishedForLookup,
      IsCollection = field.IsCollection,
      IsLocalized = field.IsLocalized,
      IsNullable = field.IsNullable,
      ReferencedEntityTypeIds = [.. field.ReferencedEntityTypeIds]
    };
    _isNew = false;
    _nameError = string.Empty;
    _isDialogOpen = true;
  }

  public async Task Delete(DataModelFieldEditModel field)
  {
    Fields.Remove(field);
    if (FieldGrid is not null)
      await FieldGrid.Refresh();
  }

  private async Task Confirm()
  {
    if (_editingField is null) return;

    if (string.IsNullOrWhiteSpace(_editingField.Name))
    {
      _nameError = "Název pole je povinný.";
      return;
    }

    _nameError = string.Empty;

    if (!NeedsReferences)
      _editingField.ReferencedEntityTypeIds.Clear();

    if (_isNew)
    {
      Fields.Add(_editingField);
    }
    else
    {
      var existing = Fields.FirstOrDefault(f => f.Id == _editingField.Id);
      if (existing is not null)
      {
        existing.Name = _editingField.Name;
        existing.Label = _editingField.Label;
        existing.Description = _editingField.Description;
        existing.FieldType = _editingField.FieldType;
        existing.IsPublishedForLookup = _editingField.IsPublishedForLookup;
        existing.IsCollection = _editingField.IsCollection;
        existing.IsLocalized = _editingField.IsLocalized;
        existing.IsNullable = _editingField.IsNullable;
        existing.ReferencedEntityTypeIds = _editingField.ReferencedEntityTypeIds;
      }
    }

    _isDialogOpen = false;
    _editingField = null;

    if (FieldGrid is not null)
      await FieldGrid.Refresh();
  }

  private void Cancel()
  {
    _isDialogOpen = false;
    _editingField = null;
    _nameError = string.Empty;
  }
}
