using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts;

/// <summary>
/// Výčtový typ pro typy polí datového modelu.
/// </summary>
public enum DataModelFieldType
{
  /// <summary>
  /// Obyčejný text.
  /// </summary>
  Text = 1,

  /// <summary>
  /// Představuje textový prvek, který podporuje víceřádkový obsah.
  /// </summary>
  MultilineText = 2,

  /// <summary>
  /// Představuje výběr mezi dvěma možnostmi (Ano/Ne).
  /// </summary>
  TwoOptions = 3,

  /// <summary>
  /// Představuje celé číslo bez desetinných míst.
  /// </summary>
  WholeNumber = 4,

  /// <summary>
  /// Představuje číslo s desetinnými místy.
  /// </summary>
  DecimalNumber = 5,

  /// <summary>
  /// Představuje jedinečný identifikátor (GUID).
  /// </summary>
  UniqueIdentifier = 6,

  /// <summary>
  /// Představuje datum a čas v koordinovaném světovém čase (UTC).
  /// </summary>
  UtcDateTime = 7,

  /// <summary>
  /// Představuje odkaz na jinou entitu v datovém modelu.
  /// </summary>
  LookupEntity = 8,

  /// <summary>
  /// Představuje vnořenou entitu v datovém modelu.
  /// </summary>
  NestedEntity = 9,

  /// <summary>
  /// Představuje pouze datum bez časové složky.
  /// </summary>
  Date = 10,

  /// <summary>
  /// Představuje odkaz na soubor nebo dokument.
  /// </summary>
  FileReference = 11,

  /// <summary>
  /// Představuje měnovou hodnotu.
  /// </summary>
  CurrencyNumber = 12,

  /// <summary>
  /// Představuje jednohodnotový výběr z předdefinovaného seznamu možností.
  /// </summary>
  SingleSelectOptionSet = 13,

  /// <summary>
  /// Představuje vícehodnotový výběr z předdefinovaného seznamu možností.
  /// </summary>
  MultiSelectOptionSet = 14
}
