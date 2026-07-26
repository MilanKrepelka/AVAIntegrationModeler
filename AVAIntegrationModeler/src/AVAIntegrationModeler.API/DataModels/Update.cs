using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModels.Update;

namespace AVAIntegrationModeler.API.DataModels;

public class Update(IMediator _mediator) : Endpoint<UpdateDataModelRequest, UpdateDataModelResponse>
{
  public override void Configure()
  {
    Put(UpdateDataModelRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(UpdateDataModelRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new UpdateDataModelCommand(request.Datasource, request.DataModel), cancellationToken);

    if (result.Status == ResultStatus.NotFound) { await SendNotFoundAsync(cancellationToken); return; }
    if (result.Status == ResultStatus.Invalid)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.Identifier ?? "DataModel", error.ErrorMessage);
      await SendErrorsAsync(400, cancellationToken);
      return;
    }
    if (result.IsSuccess)
    {
      await SendOkAsync(new UpdateDataModelResponse(result.Value), cancellationToken);
      return;
    }
    AddError(string.Join("; ", result.Errors));
    await SendErrorsAsync(500, cancellationToken);
  }
}
