using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModelRecords.Create;

namespace AVAIntegrationModeler.API.DataModelRecords;

public class Create(IMediator _mediator) : Endpoint<CreateDataModelRecordRequest, Guid>
{
  public override void Configure()
  {
    Post(CreateDataModelRecordRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateDataModelRecordRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateDataModelRecordCommand(request.Datasource, request.Record), cancellationToken);

    if (result.IsSuccess)
    {
      Response = result.Value;
      await SendCreatedAtAsync<GetById>(
        new { datasource = request.Datasource, recordId = result.Value },
        Response,
        cancellation: cancellationToken);
      return;
    }
    if (result.Status == ResultStatus.Invalid)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.Identifier ?? "Record", error.ErrorMessage);
      await SendErrorsAsync(400, cancellationToken);
      return;
    }
    AddError(string.Join("; ", result.Errors));
    await SendErrorsAsync(500, cancellationToken);
  }
}
