using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.Components.Widgets.IncludedFeaturesWidget;

/// <summary>
/// Komponenta pro zobrazení seznamu zahrnutých featur
/// </summary>
public partial class IncludedFeaturesWidget : ComponentBase
{
  /// <summary>
  /// Seznam zahrnutých featur
  /// </summary>
  [Parameter]
  public IEnumerable<IncludedFeatureDTO> Features { get; set; } = [];

  /// <summary>
  /// Zobrazit detaily zahrnutých featur
  /// </summary>
  [Parameter]
  public bool ShowDetails { get; set; } = false;
}
