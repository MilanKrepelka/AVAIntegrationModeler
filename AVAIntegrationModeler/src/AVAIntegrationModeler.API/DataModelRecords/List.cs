using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModelRecords.List;

namespace AVAIntegrationModeler.API.DataModelRecords;

public class List(IMediator _mediator) : Endpoint<DataModelRecordListRequest, DataModelRecordListResponse>
{
  public override void Configure()
  {
    Get("/DataModelRecords");
    AllowAnonymous();
    Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromSeconds(5))));
  }

  public override async Task HandleAsync(DataModelRecordListRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ListDataModelRecordsQuery(request.Datasource, request.ModelId), cancellationToken);

    if (result.IsSuccess)
    {
      Response = new DataModelRecordListResponse
      {
        Records = result.Value.ToList()
      };
    }
  }
}
