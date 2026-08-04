using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.Components.Widgets.IncludedModelWidget;

/// <summary>
/// Komponenta pro zobrazení detailu zahrnutého modelu
/// </summary>
public partial class IncludedModelWidget : ComponentBase
{
  /// <summary>
  /// Zahrnutý datový model
  /// </summary>
  [Parameter]
  public IncludedDataModelDTO Model { get; set; } = new();
}
