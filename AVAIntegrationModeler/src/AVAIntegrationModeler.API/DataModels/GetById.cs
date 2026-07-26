using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModels.Get;

namespace AVAIntegrationModeler.API.DataModels;

public class GetById(IMediator _mediator) : Endpoint<GetDataModelRequest, DataModelDTO>
{
  public override void Configure()
  {
    Get(GetDataModelRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetDataModelRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetDataModelQuery(request.Datasource, request.DataModelId), cancellationToken);

    if (result.Status == ResultStatus.NotFound) { await SendNotFoundAsync(cancellationToken); return; }
    if (result.IsSuccess) Response = result.Value;
  }
}
