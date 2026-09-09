using Ardalis.Result;
using AVAIntegrationModeler.Contracts.DataModels;
using AVAIntegrationModeler.UseCases.DataModels.Compare;
using FastEndpoints;
using MediatR;

namespace AVAIntegrationModeler.API.DataModels;

/// <summary>
/// Vrátí hluboké porovnání datového modelu mezi databází a AVAPlace.
/// </summary>
public class CompareDataModelEndpoint(IMediator mediator)
  : Endpoint<GetDataModelChangesRequest, GetDataModelChangesResponse>
{
  /// <inheritdoc />
  public override void Configure()
  {
    Get("/DataModels/{DataModelId:guid}/changes");
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí porovnání datového modelu mezi databází a AVAPlace.");
  }

  /// <inheritdoc />
  public override async Task HandleAsync(GetDataModelChangesRequest request, CancellationToken ct)
  {
    var result = await mediator.Send(new CompareDataModelQuery(request.DataModelId), ct);
    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }
    if (result.IsSuccess)
    {
      Response = new GetDataModelChangesResponse { Comparison = result.Value };
      return;
    }
    await SendErrorsAsync(cancellation: ct);
  }
}
