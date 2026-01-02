using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Widgets;

public partial class SuccessErrorToast
{
  /// <summary>
  /// Titulek toastu.
  /// </summary>
  [Parameter]
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// Additional CSS class for the wrapper.
  /// </summary>
  [Parameter]
  public string? CssClass { get; set; }

  /// <summary>
  /// Zobrazí toast s danou zprávou a stylem podle úspěchu či chyby.
  /// </summary>
  /// <param name="Success">Úspěch</param>
  /// <param name="Message">Zpráva v toastu</param>
  public async Task Show(bool Success, string Message)
  {
    await this.ShowToast(Success, Message);
  }
}
