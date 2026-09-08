using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private bool _modelsOpen;
    private bool _dataModelRecordsOpen;
    private bool _scenariosOpen;

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
        UpdateOpenGroups(NavigationManager.Uri);
    }

    private void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        UpdateOpenGroups(e.Location);
        InvokeAsync(StateHasChanged);
    }

    private void UpdateOpenGroups(string uri)
    {
        _modelsOpen = uri.Contains("/datamodels/", StringComparison.OrdinalIgnoreCase);
        _dataModelRecordsOpen = uri.Contains("/datamodelrecords/", StringComparison.OrdinalIgnoreCase);
        _scenariosOpen = uri.Contains("/scenarios/", StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}
