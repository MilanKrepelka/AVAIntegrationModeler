using Ardalis.SharedKernel;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Core.ContributorAggregate;
using AVAIntegrationModeler.UseCases.Scenarios.Create;
using FastEndpoints;

namespace AVAIntegrationModeler.UseCases.Features.List;

public record ListFeaturesQuery(Datasource Datasource, int? Skip, int? Take) : IQuery<Result<IEnumerable<FeatureDTO>>>;
public record ListFeaturesQuery2(Datasource Datasource, int? Skip, int? Take) : FastEndpoints.ICommand<Result<IEnumerable<FeatureDTO>>>;

public class ListFeaturesQueryHandler2 : CommandHandler<ListFeaturesQuery2, Result<IEnumerable<FeatureDTO>>>
{
  private readonly IListFeaturesQueryService _query;

  public ListFeaturesQueryHandler2(IListFeaturesQueryService query)
  {
    _query = query;
  }
  public override async Task<Result<IEnumerable<FeatureDTO>>> ExecuteAsync(ListFeaturesQuery2 request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync(request.Datasource);

    Console.WriteLine($"<<<<<<<Listed {result.Count()} scenarios");

    return Result.Success(result);
  }
}
