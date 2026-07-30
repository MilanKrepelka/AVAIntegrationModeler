using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModelRecords.Delete;

namespace AVAIntegrationModeler.API.DataModelRecords;

public class Delete(IMediator _mediator) : Endpoint<DeleteDataModelRecordRequest, bool>
{
  public override void Configure()
  {
    Delete(DeleteDataModelRecordRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(DeleteDataModelRecordRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DeleteDataModelRecordCommand(request.Datasource, request.RecordId), cancellationToken);

    if (result.Status == ResultStatus.NotFound) { await SendNotFoundAsync(cancellationToken); return; }
    if (result.IsSuccess) { await SendNoContentAsync(cancellationToken); return; }
    await SendErrorsAsync(400, cancellationToken);
  }
}
