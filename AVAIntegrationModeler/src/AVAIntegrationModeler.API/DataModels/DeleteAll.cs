using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModels.DeleteAll;

namespace AVAIntegrationModeler.API.DataModels;

/// <summary>
/// Endpoint pro smazání všech datových modelů z lokální databáze včetně jejich záznamů.
/// </summary>
public class DeleteAll(IMediator _mediator) : EndpointWithoutRequest<DeleteAllDataModelsResponse>
{
  public override void Configure()
  {
    Delete("/DataModels/All");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new DeleteAllDataModelsCommand(), cancellationToken);

    if (result.IsSuccess)
    {
      await SendOkAsync(result.Value, cancellationToken);
      return;
    }
    AddError(string.Join("; ", result.Errors));
    await SendErrorsAsync(500, cancellationToken);
  }
}
