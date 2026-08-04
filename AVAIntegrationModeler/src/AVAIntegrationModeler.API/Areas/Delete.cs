using AVAIntegrationModeler.UseCases.Areas.Delete;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Smaže oblast.
/// </summary>
public class DeleteAreaEndpoint(IMediator mediator) : Endpoint<DeleteAreaRequest, bool>
{
  public override void Configure()
  {
    Delete(DeleteAreaRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Smaže oblast podle kódu.");
  }

  public override async Task HandleAsync(DeleteAreaRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new DeleteAreaCommand(request.AreaCode), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
      return;
    }

    await SendErrorsAsync(400, cancellationToken);
  }
}
