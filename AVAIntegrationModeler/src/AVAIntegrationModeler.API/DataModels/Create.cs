using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModels.Create;

namespace AVAIntegrationModeler.API.DataModels;

public class Create(IMediator _mediator) : Endpoint<CreateDataModelRequest, CreateDataModelResponse>
{
  public override void Configure()
  {
    Post(CreateDataModelRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CreateDataModelRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new CreateDataModelCommand(request.Datasource, request.DataModel), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new CreateDataModelResponse { DataModelId = result.Value };
      await SendCreatedAtAsync<GetById>(
        new { datasource = request.Datasource, dataModelId = result.Value },
        Response,
        cancellation: cancellationToken);
      return;
    }
    if (result.Status == ResultStatus.Invalid)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.Identifier ?? "DataModel", error.ErrorMessage);
      await SendErrorsAsync(400, cancellationToken);
      return;
    }
    AddError(string.Join("; ", result.Errors));
    await SendErrorsAsync(500, cancellationToken);
  }
}
