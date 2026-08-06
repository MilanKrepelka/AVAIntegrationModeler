using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModels.Import;

namespace AVAIntegrationModeler.API.DataModels;

/// <summary>
/// Endpoint pro hromadný import všech datových modelů z AVAPlace do lokální databáze.
/// </summary>
public class ImportAllFromAVAPlace(IMediator _mediator)
  : EndpointWithoutRequest<ImportAllDataModelsFromAvaPlaceResponse>
{
  public override void Configure()
  {
    Post(ImportAllDataModelsFromAvaPlaceRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ImportAllDataModelsCommand(), cancellationToken);

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
