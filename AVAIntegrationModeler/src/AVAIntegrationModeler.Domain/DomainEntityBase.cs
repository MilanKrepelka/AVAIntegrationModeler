namespace AVAIntegrationModeler.Domain;

/// <summary>
/// Základní třída pro doménové entity s Guid identifikátorem a podporou záznamu data posledního uložení do databáze.
/// </summary>
public abstract class DomainEntityBase<TId> : EntityBase<TId> where TId : struct, IEquatable<TId>
{
    /// <summary>
    /// Datum a čas vytvoření entity v databázi (UTC). Nastavuje se jednorázově při prvním uložení.
    /// </summary>
    public DateTime? CreatedAt { get; private set; }

    /// <summary>
    /// Datum a čas posledního uložení entity do databáze (UTC).
    /// </summary>
    public DateTime? LastSavedAt { get; private set; }

    /// <summary>
    /// Nastaví <see cref="CreatedAt"/> na aktuální UTC čas. Volá se automaticky v <c>AppDbContext.SaveChangesAsync</c> při přidání entity.
    /// </summary>
    public void MarkCreated() => CreatedAt = DateTime.UtcNow;

    /// <summary>
    /// Nastaví <see cref="LastSavedAt"/> na aktuální UTC čas. Volá se automaticky v <c>AppDbContext.SaveChangesAsync</c>.
    /// </summary>
    public void MarkSaved() => LastSavedAt = DateTime.UtcNow;
}

/// <summary>
/// Základní třída pro doménové entity s celočíselným identifikátorem a podporou záznamu data posledního uložení do databáze.
/// </summary>
public abstract class DomainEntityBase : EntityBase
{
    /// <summary>
    /// Datum a čas vytvoření entity v databázi (UTC). Nastavuje se jednorázově při prvním uložení.
    /// </summary>
    public DateTime? CreatedAt { get; private set; }

    /// <summary>
    /// Datum a čas posledního uložení entity do databáze (UTC).
    /// </summary>
    public DateTime? LastSavedAt { get; private set; }

    /// <summary>
    /// Nastaví <see cref="CreatedAt"/> na aktuální UTC čas. Volá se automaticky v <c>AppDbContext.SaveChangesAsync</c> při přidání entity.
    /// </summary>
    public void MarkCreated() => CreatedAt = DateTime.UtcNow;

    /// <summary>
    /// Nastaví <see cref="LastSavedAt"/> na aktuální UTC čas. Volá se automaticky v <c>AppDbContext.SaveChangesAsync</c>.
    /// </summary>
    public void MarkSaved() => LastSavedAt = DateTime.UtcNow;
}
