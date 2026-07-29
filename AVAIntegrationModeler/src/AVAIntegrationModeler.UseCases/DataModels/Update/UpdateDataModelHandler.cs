using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate.Specifications;
using AVAIntegrationModeler.UseCases.DataModels.Mapping;

namespace AVAIntegrationModeler.UseCases.DataModels.Update;

public class UpdateDataModelHandler(IDataModelRepository repository, IDataModelQueryService queryService)
  : ICommandHandler<UpdateDataModelCommand, Result<DataModelDTO>>
{
  public async Task<Result<DataModelDTO>> Handle(UpdateDataModelCommand request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request.DataModel, nameof(request.DataModel));

    var existing = await repository.FirstOrDefaultAsync(
      new DataModelByIdWithFieldsSpec(request.DataModel.Id), cancellationToken);
    if (existing is null) return Result<DataModelDTO>.NotFound();

    try
    {
      existing
        .SetCode(request.DataModel.Code)
        .SetName(request.DataModel.Name)
        .SetDescription(request.DataModel.Description)
        .SetNotes(request.DataModel.Notes);

      if (request.DataModel.IsAggregateRoot)
        existing.MarkAsAggregateRoot();
      else
        existing.MarkAsNestedEntity();

      existing.SetArea(request.DataModel.AreaId);
    }
    catch (ArgumentException ex)
    {
      return Result<DataModelDTO>.Invalid(new ValidationError(ex.ParamName ?? "DataModel", ex.Message));
    }

    var fieldsToDelete = existing.Fields.ToList();

    foreach (var name in fieldsToDelete.Select(f => f.Name))
      existing.RemoveField(name);

    var fieldsToAdd = new List<DataModelField>();
    foreach (var fieldDto in request.DataModel.Fields)
    {
      try
      {
        var field = new DataModelField(Guid.NewGuid(), fieldDto.Name, fieldDto.FieldType);
        if (!string.IsNullOrEmpty(fieldDto.Label))
          field.SetLabel(fieldDto.Label);
        field.SetDescription(fieldDto.Description);
        if (fieldDto.IsPublishedForLookup) field.MarkAsPublishedForLookup();
        if (fieldDto.IsCollection) field.MarkAsCollection();
        if (fieldDto.IsLocalized) field.MarkAsLocalized();
        if (fieldDto.IsNullable) field.MarkAsNullable();
        if (fieldDto.FieldType is DataModelFieldType.LookupEntity or DataModelFieldType.NestedEntity)
          foreach (var refId in fieldDto.ReferencedEntityTypeIds)
            field.AddReferencedEntityType(refId);
        existing.AddField(field);
        fieldsToAdd.Add(field);
      }
      catch (ArgumentException ex)
      {
        return Result<DataModelDTO>.Invalid(new ValidationError(fieldDto.Name, ex.Message));
      }
    }

    await repository.SyncFieldsAndSaveAsync(existing, fieldsToDelete, fieldsToAdd, cancellationToken);
    queryService.InvalidateCache(request.Datasource);
    return Result<DataModelDTO>.Success(DataModelMapper.MapToDataModelDTO(existing));
  }
}
