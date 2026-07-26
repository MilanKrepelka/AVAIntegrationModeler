using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.UseCases.DataModels.Mapping;

namespace AVAIntegrationModeler.UseCases.DataModels.Get;

public class GetDataModelHandler(IRepository<DataModel> repository)
  : IQueryHandler<GetDataModelQuery, Result<DataModelDTO>>
{
  public async Task<Result<DataModelDTO>> Handle(GetDataModelQuery request, CancellationToken cancellationToken)
  {
    var dataModel = await repository.GetByIdAsync(request.DataModelId, cancellationToken);
    if (dataModel is null) return Result<DataModelDTO>.NotFound();
    return Result<DataModelDTO>.Success(DataModelMapper.MapToDataModelDTO(dataModel));
  }
}
