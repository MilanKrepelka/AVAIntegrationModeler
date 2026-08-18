using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.Domain.DataModelAggregate.Specifications;

/// <summary>
/// Specifikace pro vyhledání datového modelu podle kódu. Porovnání kódu je case-insensitive.
/// </summary>
public class DataModelByCodeSpec : Specification<DataModel>
{
  public DataModelByCodeSpec(string code) =>
    Query.Where(dataModel => dataModel.Code.ToUpper() == code.ToUpper());
}
