using AVAIntegrationModeler.API.DataModels;
using AVAIntegrationModeler.API.Scenarios;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;
using static MudBlazor.CategoryTypes;
using static MudBlazor.Colors;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace AVAIntegrationModeler.Web.Components.Pages;

public partial class DataModels : Microsoft.AspNetCore.Components.ComponentBase, IPageListBase
{
  /// <inheritdoc/>
  public bool IsLoading { get; set; } = false;
  /// <inheritdoc/>
  public Datasource Datasource { get; set; } = Datasource.Database;
  /// <inheritdoc/>
  public string FilterString { get; set; } = string.Empty;

  public List<DataModelListViewModel> DataModelList { get; set; } = new();

  protected async Task onDatasourceChanded(Datasource datasource)
  {
    this.Datasource = datasource;
    await LoadItemsAsync();
  }

  

  protected bool FilterFunc(DataModelListViewModel dataModel)
  {
    if (string.IsNullOrEmpty(FilterString)) return true;

    return dataModel.Code.Contains(FilterString, StringComparison.OrdinalIgnoreCase)
      || dataModel.Id.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)

      || dataModel.Name.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)
      || dataModel.Notes.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)
      || dataModel.Description.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)
      || dataModel.Fields.Any(field => field.Name.Contains(FilterString, StringComparison.OrdinalIgnoreCase))
//      || dataModel.Fields.Any(field => field.Description.Contains(FilterString, StringComparison.OrdinalIgnoreCase));
//      || dataModel.Fields.Any(field => field.ReferencedModels.Any(referencedModel=>referencedModel.Code.Contains(FilterString, StringComparison.OrdinalIgnoreCase)));
  }
  protected async Task LoadItemsAsync()
  {
    try
    {
      IsLoading = true;
      DataModelList.Clear();
      // Načtení scénářů z AVAIntegrationModeler.API
      using var httpClient = new HttpClient();
      var response = await httpClient.GetAsync($"http://localhost:57679/DataModels?datasource={this.Datasource}");
      response.EnsureSuccessStatusCode();

      var options = new JsonSerializerOptions
      {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true // Ignoruje case sensitivity
      };

      //var xxx = await response.Content.ReadFromJsonAsync<JsonObject>();
      var dataModelListResponse = await response.Content.ReadAsStringAsync();
      var dataModelListResponseObj = JsonSerializer.Deserialize<DataModelListResponse>(dataModelListResponse, options);

      if (dataModelListResponseObj?.DataModels != null)
      {
        foreach (var dataModel in dataModelListResponseObj?.DataModels!)
        {

          DataModelListViewModel? dataModelListViewModel = new DataModelListViewModel();
          dataModelListViewModel = Mapping.DataModelMapper.MapToViewModel(dataModel, dataModelListResponseObj.DataModels);
          if (dataModelListViewModel != null)DataModelList.Add(dataModelListViewModel);
        }
      }
    }
    
    catch (HttpRequestException httpEx)
    {
      Console.WriteLine($"Chyba HTTP požadavku: {httpEx.Message}");
      // fallback na lokální data
      
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání scénářů: {ex.Message}");
      // fallback na lokální data
      DataModelList = new();
    }
    finally
    {
      IsLoading = false;
    }
  }
  protected void ShowBtnPress(DataModelListViewModel dataModelListViewModel )
  {
    dataModelListViewModel.ShowDetails = !dataModelListViewModel.ShowDetails;
  }


  protected override async Task OnInitializedAsync()
  {
    base.OnInitialized();
    await LoadItemsAsync();
  }
}
