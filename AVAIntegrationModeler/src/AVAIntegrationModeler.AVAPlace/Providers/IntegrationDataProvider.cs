using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASOL.Core.Identity;
using ASOL.Core.Identity.Options;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlace.Options;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RTX = AVAIntegrationModeler.AVAPlace.ServiceRuntimeTenantContext;

namespace AVAIntegrationModeler.AVAPlace.Providers;

/// <summary>
/// Implementace poskytovatele dat pro integrace. <see cref="IIntegrationDataProvider"/>
/// </summary>
public class IntegrationDataProvider : IIntegrationDataProvider
{
  private readonly IServiceProvider _serviceProvider;
  private readonly IOptions<AVAPlaceOptions> _avaPlaceOptions;
  private readonly IRuntimeContext _runtimeContext;
  

  /// <summary>
  /// Základní konstruktor.
  /// </summary>
  /// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
  /// <exception cref="ArgumentNullException"></exception>
  public IntegrationDataProvider(
    IServiceProvider serviceProvider,
    IOptions< Options.AVAPlaceOptions> avaPlaceOptions
    )
  {
    _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    _avaPlaceOptions = avaPlaceOptions;
    _runtimeContext = _serviceProvider.GetRequiredService<IRuntimeContext>() ?? throw new ArgumentNullException(nameof(IRuntimeContext));
  }

  public Task<IEnumerable<FeatureDTO>> GetIntegrationFeaturesAsync(CancellationToken ct = default)
  {
    throw new NotImplementedException();
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<ScenarioDTO>> GetIntegrationScenariosAsync(CancellationToken ct = default)
  {

    // Assuming you have access to a serviceProvider and tenantId in your context.
    // You may need to inject these via constructor or other means.

    string tenantId =  _runtimeContext?.Security?.TenantId!;
    if (string.IsNullOrEmpty(tenantId))
    {
      tenantId = _avaPlaceOptions.Value.TenantId;
    };

    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IEnumerable<ScenarioDTO>>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        List<ScenarioDTO> scenarios = new List<ScenarioDTO>();
        // Replace with actual logic to get scenarios, e.g.:
        var dataServiceResult = await client.IntegrationScenarios.GetScenariosAsync(
         new ASOL.DataService.Contracts.Filters.IntegrationScenarioFilter()
         {
         },
         new ASOL.Core.Paging.Contracts.Filters.PagingFilter()
         {
           Offset = 0,
           Limit = int.MaxValue
         }, new ASOL.Core.Domain.Contracts.BaseEntityFilter()
         {
           Released = true,
           Deleted = false,
         },
         ct);
        if (dataServiceResult != null && dataServiceResult.Any())
        {
          foreach (var scenario in dataServiceResult)
          {
           scenarios.Add(Mapping.ScenarioMapper.MapToDTO(scenario));
          }
        }
          return scenarios;
      }
    );
  }
}
