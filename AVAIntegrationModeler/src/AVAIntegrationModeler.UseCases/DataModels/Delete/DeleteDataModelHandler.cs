using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.UseCases.DataModels.Delete;

public class DeleteDataModelHandler(IRepository<DataModel> repository)
  : ICommandHandler<DeleteDataModelCommand, Result>
{
  public async Task<Result> Handle(DeleteDataModelCommand request, CancellationToken cancellationToken)
  {
    var dataModel = await repository.GetByIdAsync(request.DataModelId, cancellationToken);
    if (dataModel is null) return Result.NotFound();

    await repository.DeleteAsync(dataModel, cancellationToken);
    return Result.Success();
  }
}
