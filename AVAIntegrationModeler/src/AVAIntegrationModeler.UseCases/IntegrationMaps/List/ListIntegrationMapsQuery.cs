using Ardalis.SharedKernel;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.IntegrationMapAggregate;
using AVAIntegrationModeler.UseCases.IntegrationMaps.List;
using FastEndpoints;

namespace AVAIntegrationModeler.UseCases.IntegrationMaps.List;

public record ListIntegrationMapsQuery(Datasource Datasource, int? Skip, int? Take) : IQuery<Result<IEnumerable<IntegrationMapSummaryDTO>>>;
public record ListIntegrationMapsQuery2(Datasource Datasource, int? Skip, int? Take) : FastEndpoints.ICommand<Result<IEnumerable<IntegrationMapSummaryDTO>>>;

public class ListIntegrationMapsQueryHandler2 : CommandHandler<ListIntegrationMapsQuery2, Result<IEnumerable<IntegrationMapSummaryDTO>>>
{
  private readonly IListIntegrationMapsQueryService _query;

  public ListIntegrationMapsQueryHandler2(IListIntegrationMapsQueryService query)
  {
    _query = query;
  }
  public override async Task<Result<IEnumerable<IntegrationMapSummaryDTO>>> ExecuteAsync(ListIntegrationMapsQuery2 request, CancellationToken cancellationToken)
  {
    var result = await _query.ListSummaryAsync(request.Datasource);

    Console.WriteLine($"<<<<<<<Listed {result.Count()} scenarios");

    return Result.Success(result);
  }
}
