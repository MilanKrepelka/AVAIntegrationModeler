using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.ViewModels.List;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.Components.Widgets.DataModelFieldWidget;

/// <summary>
/// Komponenta pro zobrazení seznamu polí datového modelu
/// </summary>
public partial class DataModelFieldWidget : ComponentBase
{
    /// <summary>
    /// Seznam polí datového modelu
    /// </summary>
    [Parameter]
    public IEnumerable<DataModelFieldListViewModel> Fields { get; set; } = [];

    /// <summary>
    /// Zobrazit detaily polí
    /// </summary>
    [Parameter]
    public bool ShowDetails { get; set; } = false;

    /// <summary>
    /// Získá lokalizovaný název typu pole
    /// </summary>
    private string GetFieldTypeDisplayName(DataModelFieldType fieldType)
    {
        return fieldType switch
        {
            DataModelFieldType.Text => "Text",
            DataModelFieldType.MultilineText => "Víceřádkový text",
            DataModelFieldType.TwoOptions => "Ano/Ne",
            DataModelFieldType.WholeNumber => "Celé číslo",
            DataModelFieldType.DecimalNumber => "Desetinné číslo",
            DataModelFieldType.UniqueIdentifier => "GUID",
            DataModelFieldType.UtcDateTime => "Datum a čas (UTC)",
            DataModelFieldType.LookupEntity => "Odkaz na entitu",
            DataModelFieldType.NestedEntity => "Vnořená entita",
            DataModelFieldType.Date => "Datum",
            DataModelFieldType.FileReference => "Odkaz na soubor",
            DataModelFieldType.CurrencyNumber => "Měna",
            DataModelFieldType.SingleSelectOptionSet => "Výběr (jednoduchý)",
            DataModelFieldType.MultiSelectOptionSet => "Výběr (vícenásobný)",
            _ => fieldType.ToString()
        };
    }
}
