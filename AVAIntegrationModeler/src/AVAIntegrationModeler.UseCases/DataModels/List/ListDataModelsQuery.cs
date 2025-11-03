using Ardalis.SharedKernel;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.UseCases.Scenarios.Create;
using FastEndpoints;

namespace AVAIntegrationModeler.UseCases.DataModels.List;

public record ListDataModelsQuery(Datasource Datasource, int? Skip, int? Take) : IQuery<Result<IEnumerable<DataModelDTO>>>;
public record ListDataModelsQuery2(Datasource Datasource, int? Skip, int? Take) : FastEndpoints.ICommand<Result<IEnumerable<DataModelDTO>>>;

public class ListDataModelsQueryHandler2(IListDataModelQueryService query) : CommandHandler<ListDataModelsQuery2, Result<IEnumerable<DataModelDTO>>>
{
  private readonly IListDataModelQueryService _query = query;

  public override async Task<Result<IEnumerable<DataModelDTO>>> ExecuteAsync(ListDataModelsQuery2 request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync(request.Datasource);

    Console.WriteLine($"<<<<<<<Listed {result.Count()} scenarios");

    return Result.Success(result);
  }
}
