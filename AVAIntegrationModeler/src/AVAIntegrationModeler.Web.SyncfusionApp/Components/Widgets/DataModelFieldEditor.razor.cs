using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Widgets;

/// <summary>
/// Editační model pole datového modelu používaný v dialogu <see cref="DataModelFieldEditor"/>.
/// </summary>
public class DataModelFieldEditModel
{
  /// <summary>
  /// Validační hláška pro chybějící název pole.
  /// </summary>
  public const string NameRequiredMessage = "Název pole je povinný.";

  /// <summary>
  /// Validační hláška pro chybějící popisek pole.
  /// </summary>
  public const string LabelRequiredMessage = "Popisek pole je povinný.";

  /// <summary>
  /// Validační hláška pro chybějící popis pole.
  /// </summary>
  public const string DescriptionRequiredMessage = "Popis pole je povinný.";

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
  public string ExpressionValue { get; set; } = string.Empty;
  public int? ExpressionOrder { get; set; }

  /// <summary>
  /// Vrátí validační chybu názvu pole, nebo prázdný řetězec, pokud je název vyplněný.
  /// </summary>
  public string GetNameError()
    => string.IsNullOrWhiteSpace(Name) ? NameRequiredMessage : string.Empty;

  /// <summary>
  /// Vrátí validační chybu popisku pole, nebo prázdný řetězec, pokud je popisek vyplněný.
  /// </summary>
  public string GetLabelError()
    => string.IsNullOrWhiteSpace(Label) ? LabelRequiredMessage : string.Empty;

  /// <summary>
  /// Vrátí validační chybu popisu pole, nebo prázdný řetězec, pokud je popis vyplněný.
  /// </summary>
  public string GetDescriptionError()
    => string.IsNullOrWhiteSpace(Description) ? DescriptionRequiredMessage : string.Empty;
}

public partial class DataModelFieldEditor : ComponentBase
{
  [Parameter] public List<DataModelFieldEditModel> Fields { get; set; } = [];
  [Parameter] public List<DataModelSummaryDTO> AvailableModels { get; set; } = [];

  /// <summary>
  /// Vyvolá se po přidání nového pole nebo po smazání pole (ne po editaci existujícího) —
  /// typicky navázáno na uložení rodičovského DataModelu, aby se změna hned persistovala.
  /// </summary>
  [Parameter] public EventCallback OnFieldsChanged { get; set; }

  private SfGrid<DataModelFieldEditModel>? FieldGrid;
  private List<DataModelFieldEditModel>? _previousFields;

  private DataModelFieldEditModel? _editingField;
  private bool _isNew;
  private bool _isDialogOpen;
  private string _nameError = string.Empty;
  private string _labelError = string.Empty;
  private string _descriptionError = string.Empty;

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

  private void ToggleReferencedModel(Guid modelId, bool isChecked)
  {
    if (_editingField is null) return;
    if (isChecked && !_editingField.ReferencedEntityTypeIds.Contains(modelId))
      _editingField.ReferencedEntityTypeIds.Add(modelId);
    else if (!isChecked)
      _editingField.ReferencedEntityTypeIds.Remove(modelId);
  }

  protected override void OnParametersSet()
  {
    _previousFields = Fields;
  }

  public void OpenAdd()
  {
    _editingField = new DataModelFieldEditModel();
    _isNew = true;
    _nameError = string.Empty;
    _labelError = string.Empty;
    _descriptionError = string.Empty;
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
      ReferencedEntityTypeIds = [.. field.ReferencedEntityTypeIds],
      ExpressionValue = field.ExpressionValue,
      ExpressionOrder = field.ExpressionOrder
    };
    _isNew = false;
    _nameError = string.Empty;
    _labelError = string.Empty;
    _descriptionError = string.Empty;
    _isDialogOpen = true;
  }

  public async Task Delete(DataModelFieldEditModel field)
  {
    Fields.Remove(field);
    if (FieldGrid is not null)
      await FieldGrid.Refresh();

    if (OnFieldsChanged.HasDelegate)
      await OnFieldsChanged.InvokeAsync();
  }

  private async Task Confirm()
  {
    if (_editingField is null) return;

    _nameError = _editingField.GetNameError();
    _labelError = _editingField.GetLabelError();
    _descriptionError = _editingField.GetDescriptionError();
    if (_nameError.Length > 0 || _labelError.Length > 0 || _descriptionError.Length > 0)
      return;

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
        existing.ExpressionValue = _editingField.ExpressionValue;
        existing.ExpressionOrder = _editingField.ExpressionOrder;
      }
    }

    var wasNew = _isNew;

    _isDialogOpen = false;
    _editingField = null;

    if (FieldGrid is not null)
      await FieldGrid.Refresh();

    if (wasNew && OnFieldsChanged.HasDelegate)
      await OnFieldsChanged.InvokeAsync();
  }

  private void Cancel()
  {
    _isDialogOpen = false;
    _editingField = null;
    _nameError = string.Empty;
    _labelError = string.Empty;
    _descriptionError = string.Empty;
  }
}
