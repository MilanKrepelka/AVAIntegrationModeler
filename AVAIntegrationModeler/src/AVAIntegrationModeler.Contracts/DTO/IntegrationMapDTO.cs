using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts.DTO;
/// <summary>
/// DTO pro integrační mapu.
/// </summary>
public class IntegrationMapDTO
{
    /// <summary>
    /// Identifikátor mapy.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Název mapy.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Popis mapy.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Položky integrace v mapě.
    /// </summary>
    public List<MapItemDTO> Items { get; set; } = new();

    /// <summary>
    /// Datum vytvoření.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Datum poslední aktualizace.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO pro položku integrační mapy.
/// </summary>
public class MapItemDTO
{
    /// <summary>
    /// Identifikátor položky.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifikátor mapy, do které položka patří.
    /// </summary>
    public Guid MapId { get; set; }

    /// <summary>
    /// Název položky.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Typ položky (např. System, Process, DataFlow).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Pozice X na mapě.
    /// </summary>
    public double PositionX { get; set; }

    /// <summary>
    /// Pozice Y na mapě.
    /// </summary>
    public double PositionY { get; set; }

    /// <summary>
    /// Metadata položky (JSON).
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Připojení k jiným položkám.
    /// </summary>
    public List<Guid> ConnectedItemIds { get; set; } = new();
}
