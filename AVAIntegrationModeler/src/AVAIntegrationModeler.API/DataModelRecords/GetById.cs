using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModelRecords.Get;

namespace AVAIntegrationModeler.API.DataModelRecords;

public class GetById(IMediator _mediator) : Endpoint<GetDataModelRecordRequest, DataModelRecordDTO>
{
  public override void Configure()
  {
    Get(GetDataModelRecordRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetDataModelRecordRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new GetDataModelRecordQuery(request.Datasource, request.RecordId), cancellationToken);

    if (result.Status == ResultStatus.NotFound) { await SendNotFoundAsync(cancellationToken); return; }
    if (result.IsSuccess) Response = result.Value;
  }
}
