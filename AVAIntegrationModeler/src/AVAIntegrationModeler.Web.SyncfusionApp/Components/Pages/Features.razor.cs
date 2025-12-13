using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class Features : ComponentBase, IPageListBase
{
    /// <inheritdoc/>
    public bool IsLoading { get; set; } = false;
    
    /// <inheritdoc/>
    public Datasource Datasource { get; set; } = Datasource.Database;
    
    /// <inheritdoc/>
    public string FilterString { get; set; } = string.Empty;

    public List<FeatureListViewModel> FeaturesList { get; set; } = new();

    protected async Task LoadItemsAsync()
    {
        try
        {
            IsLoading = true;
            FeaturesList.Clear();
            
            // Načtení features z AVAIntegrationModeler.API
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://localhost:57679/Features?datasource={Datasource.AVAPlace}");
            response.EnsureSuccessStatusCode();

            var featureListResponse = await response.Content.ReadFromJsonAsync<FeatureListResponse>();
            
            if (featureListResponse?.Features != null)
            {
                foreach (var feature in featureListResponse.Features)
                {
                    FeatureListViewModel? featureListViewModel = Mapping.FeatureMapper.MapToViewModel(feature);
                    if (featureListViewModel != null) 
                    {
                        FeaturesList.Add(featureListViewModel);
                    }
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"Chyba HTTP požadavku: {httpEx.Message}");
            FeaturesList = new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chyba při načítání features: {ex.Message}");
            FeaturesList = new();
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadItemsAsync();
    }
}
