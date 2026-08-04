using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.Components.Widgets.IncludedModelsWidget;

/// <summary>
/// Komponenta pro zobrazení seznamu zahrnutých modelů
/// </summary>
public partial class IncludedModelsWidget : ComponentBase
{
  /// <summary>
  /// Seznam zahrnutých modelů
  /// </summary>
  [Parameter]
  public List<IncludedDataModelDTO> Models { get; set; } = [];

  /// <summary>
  /// Zobrazit detailní informace
  /// </summary>
  [Parameter]
  public bool ShowDetails { get; set; } = false;
}
