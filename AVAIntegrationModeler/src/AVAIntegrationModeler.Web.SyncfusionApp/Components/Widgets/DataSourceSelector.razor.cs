using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Widgets;

public partial class DataSourceSelector
{
  private string? _checked;

  /// <summary>
  /// Currently selected data source. Supports two-way binding via bind-Selected.
  /// Values: "Database" or "AVAPlace"
  /// </summary>
  [Parameter]
  public string? Selected { get; set; } = "Database";

  /// <summary>
  /// EventCallback used for bind-Selected
  /// </summary>
  [Parameter]
  public EventCallback<string?> SelectedChanged { get; set; }

  /// <summary>
  /// Optional callback invoked when selection changes.
  /// </summary>
  [Parameter]
  public EventCallback<Contracts.Datasource> OnChanged { get; set; }

  /// <summary>
  /// Label text shown before radio buttons.
  /// </summary>
  [Parameter]
  public string LabelText { get; set; } = "Zdroj dat:";

  /// <summary>
  /// Additional CSS class for the wrapper.
  /// </summary>
  [Parameter]
  public string? CssClass { get; set; }

  protected override void OnParametersSet()
  {
    // Initialize internal checked value from parameter
    _checked = Selected ?? "Database";
  }

  private async Task OnRadioChanged(ChangeEventArgs args)
  {
    var newValue = args?.Value?.ToString() ?? _checked;
    _checked = newValue;

    // Notify two-way binding target
    if (SelectedChanged.HasDelegate)
    {
      await SelectedChanged.InvokeAsync(newValue);
    }
    else
    {
      // fallback: update Selected locally
      Selected = newValue;
    }

    // Optional additional callback
    if (OnChanged.HasDelegate)
    {
      await OnChanged.InvokeAsync((newValue ?? string.Empty) == "Database" ? Contracts.Datasource.Database : Contracts.Datasource.AVAPlace);
    }
  }
}
