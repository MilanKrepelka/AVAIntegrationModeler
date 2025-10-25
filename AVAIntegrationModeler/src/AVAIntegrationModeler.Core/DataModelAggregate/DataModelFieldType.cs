using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Core.DataModelAggregate;
/// <summary>
/// Výčtový typ pro typy polí datového modelu.
/// </summary>
public enum DataModelFieldType
{
  /// <summary>
  /// Obyčejný text
  /// </summary>
  Text = 1,
  /// <summary>
  /// Představuje textový prvek, který podporuje víceřádkový obsah.
  /// </summary>
  /// <remarks>Tento typ se obvykle používá k zpracování a zobrazování textu, který přesahuje více řádků, například v
  /// textových editorech, víceřádkových vstupních polích nebo při formátovaném zobrazování textu.</remarks>
  MultilineText,
  /// <summary>
  /// Představuje výběr mezi dvěma možnostmi.
  /// </summary>
  /// <remarks>Tento typ se obvykle používá k modelování binárního rozhodnutí nebo přepínače, například "Ano/Ne" nebo
  /// "Povoleno/Zakázáno".</remarks>
  TwoOptions,
  /// <summary>
  /// Představuje celé číslo bez desetinných míst.
  /// </summary>
  WholeNumber,
  /// <summary>
  /// Představuje číslo s desetinnými místy.
  /// </summary>
  DecimalNumber,
  /// <summary>
  /// Představuje jedinečný identifikátor (GUID).
  /// </summary>
  UniqueIdentifier,
  /// <summary>
  /// Představuje datum a čas v koordinovaném světovém čase (UTC).
  /// </summary>
  UtcDateTime,
  /// <summary>
  /// Představuje odkaz na jinou entitu v datovém modelu.
  /// </summary>
  LookupEntity,
  /// <summary>
  /// Představuje vnořenou entitu v datovém modelu.
  /// </summary>
  NestedEntity,
  /// <summary>
  /// Představuje pouze datum bez časové složky.
  /// </summary>
  Date,
  /// <summary>
  /// Představuje odkaz na soubor nebo dokument.
  /// </summary>
  FileReference,
  /// <summary>
  /// Představuje měnovou hodnotu.
  /// </summary>
  CurrencyNumber,
  /// <summary>
  /// Představuje jednohodnotový výběr z předdefinovaného seznamu možností.
  /// </summary>
  SingleSelectOptionSet,
  /// <summary>
  /// Představuje vícehodnotový výběr z předdefinovaného seznamu možností.
  /// </summary>
  MultiSelectOptionSet
}
