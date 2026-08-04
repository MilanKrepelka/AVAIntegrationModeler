using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.Areas.Create;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Vytvoří novou oblast.
/// </summary>
public class CreateAreaEndpoint(IMediator mediator) : Endpoint<CreateAreaRequest, Guid>
{
  public override void Configure()
  {
    Post(CreateAreaRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Vytvoří novou oblast.";
      s.Description = "Vytvoří novou oblast v zadaném datasource. Vyžaduje validní AreaDTO.";
      s.ExampleRequest = new CreateAreaRequest
      {
        Datasource = Datasource.Database,
        Area = new Contracts.DTO.AreaDTO { Code = "AREA-001", Name = "Testovací oblast" }
      };
    });
  }

  public override async Task HandleAsync(CreateAreaRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new CreateAreaCommand(request.Datasource, request.Area), cancellationToken);

    if (result.IsSuccess)
    {
      Response = result.Value;
      await SendCreatedAtAsync<GetAreaByIdEndpoint>(
        new { datasource = request.Datasource, areaId = result.Value },
        Response,
        cancellation: cancellationToken);
      return;
    }

    if (result.Status == ResultStatus.Invalid)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.Identifier ?? "Area", error.ErrorMessage);
      await SendErrorsAsync(400, cancellationToken);
      return;
    }

    if (result.Status == ResultStatus.Conflict)
    {
      AddError(string.Join("; ", result.Errors));
      await SendErrorsAsync(409, cancellationToken);
      return;
    }

    AddError(string.Join("; ", result.Errors));
    await SendErrorsAsync(500, cancellationToken);
  }
}
