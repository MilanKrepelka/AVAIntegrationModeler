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

   
}
