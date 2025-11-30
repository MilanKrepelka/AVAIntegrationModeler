using AVAIntegrationModeler.API.DataModels;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class DataModels : ComponentBase
{
  

  /// <inheritdoc/>
  public bool IsLoading { get; set; } = false;
    
    /// <inheritdoc/>
    public Datasource Datasource { get; set; } = Datasource.Database;
    
    /// <inheritdoc/>
    public string FilterString { get; set; } = string.Empty;

    public List<DataModelListViewModel> DataModelList { get; set; } = new();

    protected async Task LoadItemsAsync()
    {
        try
        {
            IsLoading = true;
            DataModelList.Clear();
            
            // Načtení data modelů z AVAIntegrationModeler.API
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://localhost:57679/DataModels?datasource={this.Datasource}");
            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                PropertyNameCaseInsensitive = true
            };

            var dataModelListResponseJson = await response.Content.ReadAsStringAsync();
            var dataModelListResponse = JsonSerializer.Deserialize<DataModelListResponse>(dataModelListResponseJson, options);
            
            if (dataModelListResponse?.DataModels != null)
            {
                foreach (var dataModel in dataModelListResponse.DataModels)
                {
                    DataModelListViewModel? dataModelListViewModel = Mapping.DataModelMapper.MapToViewModel(dataModel, dataModelListResponse.DataModels);
                    if (dataModelListViewModel != null) 
                    {
                        DataModelList.Add(dataModelListViewModel);
                    }
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"Chyba HTTP požadavku: {httpEx.Message}");
            DataModelList = new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chyba při načítání datových modelů: {ex.Message}");
            DataModelList = new();
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
