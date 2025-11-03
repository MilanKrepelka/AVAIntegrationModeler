using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.Components.Widgets.IncludedFeatureWidget;

/// <summary>
/// Komponenta pro zobrazení detailu zahrnuté featury
/// </summary>
public partial class IncludedFeatureWidget : ComponentBase
{
  /// <summary>
  /// Zahrnutá featura
  /// </summary>
  [Parameter]
  public IncludedFeatureDTO Feature { get; set; } = new();
}
