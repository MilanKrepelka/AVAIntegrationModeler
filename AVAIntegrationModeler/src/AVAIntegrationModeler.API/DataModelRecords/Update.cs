using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModelRecords.Update;

namespace AVAIntegrationModeler.API.DataModelRecords;

public class Update(IMediator _mediator) : Endpoint<UpdateDataModelRecordRequest, Guid>
{
  public override void Configure()
  {
    Put(UpdateDataModelRecordRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateDataModelRecordRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new UpdateDataModelRecordCommand(request.Datasource, request.Record), cancellationToken);

    if (result.Status == ResultStatus.NotFound) { await SendNotFoundAsync(cancellationToken); return; }
    if (result.Status == ResultStatus.Invalid)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.Identifier ?? "Record", error.ErrorMessage);
      await SendErrorsAsync(400, cancellationToken);
      return;
    }
    if (result.IsSuccess)
    {
      await SendOkAsync(result.Value, cancellationToken);
      return;
    }
    AddError(string.Join("; ", result.Errors));
    await SendErrorsAsync(500, cancellationToken);
  }
}
