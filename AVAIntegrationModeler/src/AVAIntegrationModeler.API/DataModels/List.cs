using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModels.List;
using AVAIntegrationModeler.UseCases.Features;
using AVAIntegrationModeler.UseCases.Features.List;

namespace AVAIntegrationModeler.API.DataModels;

/// <summary>
/// List all DataModels
/// </summary>
/// <remarks>
/// List all DataModels.
/// </remarks>
public class List(IMediator _mediator) : Endpoint<DataModelListRequest, DataModelListResponse>
{
  public override void Configure()
  {
    Get("/DataModels");
    AllowAnonymous();
    Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromSeconds(5))));
  }

  public override async Task HandleAsync(DataModelListRequest request, CancellationToken cancellationToken)
  {

    Result<IEnumerable<DataModelDTO>> result = await _mediator.Send(new ListDataModelsQuery(request.Datasource, null, null), cancellationToken);

    //var result2 = await new ListFeaturesQuery2(request.Datasource, null, null)
    //  .ExecuteAsync(cancellationToken);

    if (result.IsSuccess)
    {
      Response = new DataModelListResponse
      {
        DataModels = result.Value.ToList()
      };
    }
  }
}
