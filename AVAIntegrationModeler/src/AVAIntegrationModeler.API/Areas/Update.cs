using AVAIntegrationModeler.UseCases.Areas.Update;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Aktualizuje existující oblast.
/// </summary>
public class UpdateAreaEndpoint(IMediator mediator) : Endpoint<UpdateAreaRequest, UpdateAreaResponse>
{
  public override void Configure()
  {
    Put(UpdateAreaRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Aktualizuje existující oblast.");
  }

  public override async Task HandleAsync(UpdateAreaRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new UpdateAreaCommand(request.Datasource, request.Area), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.Status == ResultStatus.Invalid)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.Identifier ?? "Area", error.ErrorMessage);
      await SendErrorsAsync(400, cancellationToken);
      return;
    }

    await SendOkAsync(new UpdateAreaResponse(result.Value), cancellationToken);
  }
}
