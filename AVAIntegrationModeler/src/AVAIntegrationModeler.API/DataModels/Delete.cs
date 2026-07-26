using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModels.Delete;

namespace AVAIntegrationModeler.API.DataModels;

public class Delete(IMediator _mediator) : Endpoint<DeleteDataModelRequest, bool>
{
  public override void Configure()
  {
    Delete(DeleteDataModelRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteDataModelRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DeleteDataModelCommand(request.Datasource, request.DataModelId), cancellationToken);

    if (result.Status == ResultStatus.NotFound) { await SendNotFoundAsync(cancellationToken); return; }
    if (result.IsSuccess) { await SendNoContentAsync(cancellationToken); return; }
    await SendErrorsAsync(400, cancellationToken);
  }
}
