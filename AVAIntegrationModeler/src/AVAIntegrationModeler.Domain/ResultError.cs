using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Domain;

/// <summary>
/// Reprezentuje chybu výsledku operace.
/// </summary>
/// <param name="Code">Kód chyby</param>
/// <param name="Message">Zpráva o chyba</param>
/// <param name="Field">Označení pole ve formuláři. Lze svázat chybu a pole ve formuláři</param>
public sealed record ResultError(string Code,string Message,string? Field = null);
