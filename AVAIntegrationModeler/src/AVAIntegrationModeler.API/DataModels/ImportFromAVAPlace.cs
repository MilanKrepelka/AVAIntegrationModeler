using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModels.Import;

namespace AVAIntegrationModeler.API.DataModels;

public class ImportFromAVAPlace(IMediator _mediator) : Endpoint<ImportDataModelFromAvaPlaceRequest, Guid>
{
  public override void Configure()
  {
    Post(ImportDataModelFromAvaPlaceRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(ImportDataModelFromAvaPlaceRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ImportDataModelCommand(request.AvaPlaceModelId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }
    if (result.Status == ResultStatus.Invalid)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.Identifier ?? "Import", error.ErrorMessage);
      await SendErrorsAsync(400, cancellationToken);
      return;
    }
    if (result.IsSuccess)
    {
      Response = result.Value;
      await SendOkAsync(Response, cancellationToken);
      return;
    }
    AddError(string.Join("; ", result.Errors));
    await SendErrorsAsync(500, cancellationToken);
  }
}
