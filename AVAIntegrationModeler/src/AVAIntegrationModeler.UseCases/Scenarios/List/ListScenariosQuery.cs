using Ardalis.SharedKernel;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.UseCases.Scenarios.Create;
using FastEndpoints;

namespace AVAIntegrationModeler.UseCases.Scenarios.List;

public record ListScenariosQuery(Datasource Datasource, int? Skip, int? Take) : IQuery<Result<IEnumerable<ScenarioDTO>>>;
public record ListScenariosQuery2(Datasource Datasource, int? Skip, int? Take) : FastEndpoints.ICommand<Result<IEnumerable<ScenarioDTO>>>;

public class ListScenariosQueryHandler2 : CommandHandler<ListScenariosQuery2, Result<IEnumerable<ScenarioDTO>>>
{
  private readonly IListScenariosQueryService _query;

  public ListScenariosQueryHandler2(IListScenariosQueryService query)
  {
    _query = query;
  }
  public override async Task<Result<IEnumerable<ScenarioDTO>>> ExecuteAsync(ListScenariosQuery2 request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync(request.Datasource);

    Console.WriteLine($"<<<<<<<Listed {result.Count()} scenarios");

    return Result.Success(result);
  }
}
