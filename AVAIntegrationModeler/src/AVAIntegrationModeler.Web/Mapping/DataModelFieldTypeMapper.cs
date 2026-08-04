using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.Web.Mapping;

public static class DataModelFieldTypeMapper
{
  /// <summary>
  /// Získá lokalizovaný název typu pole
  /// </summary>
  public static string GetFieldTypeDisplayName(DataModelFieldType fieldType)
  {
    return fieldType switch
    {
      DataModelFieldType.Text => "Text",
      DataModelFieldType.MultilineText => "Víceřádkový text",
      DataModelFieldType.TwoOptions => "Ano/Ne",
      DataModelFieldType.WholeNumber => "Celé číslo",
      DataModelFieldType.DecimalNumber => "Desetinné číslo",
      DataModelFieldType.UniqueIdentifier => "GUID",
      DataModelFieldType.UtcDateTime => "Datum a čas (UTC)",
      DataModelFieldType.LookupEntity => "Odkaz na entitu",
      DataModelFieldType.NestedEntity => "Vnořená entita",
      DataModelFieldType.Date => "Datum",
      DataModelFieldType.FileReference => "Odkaz na soubor",
      DataModelFieldType.CurrencyNumber => "Měna",
      DataModelFieldType.SingleSelectOptionSet => "Výběr (jednoduchý)",
      DataModelFieldType.MultiSelectOptionSet => "Výběr (vícenásobný)",
      _ => fieldType.ToString()
    };
  }
}
