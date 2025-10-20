using AVAIntegrationModeler.Core.ContributorAggregate;
using AVAIntegrationModeler.Core.ScenarioAggregate;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace AVAIntegrationModeler.Infrastructure.Data;


public class AppDbContext : DbContext
{
  private readonly IDomainEventDispatcher? _dispatcher;
  private readonly DbContextOptions<AppDbContext>? _options;

  public AppDbContext()
  {
    // Optionally, you can assign null explicitly for clarity
    _options = null!;
    _dispatcher = null!;
  }
  public AppDbContext(DbContextOptions<AppDbContext> options,IDomainEventDispatcher? dispatcher)
  {
    this._options = options;
    this._dispatcher = dispatcher;
  }
  

  public DbSet<Contributor> Contributors => Set<Contributor>();
  public DbSet<Scenario> Scenarios => Set<Scenario>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    // Toto zaregistruje všechny konfigurace entit ve shodě s IEntityTypeConfiguration v aktuálním sestavení
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<HasDomainEventsBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      optionsBuilder.UseSqlite("Data Source=app.db");
    }
  }

  public override int SaveChanges() =>
        SaveChangesAsync().GetAwaiter().GetResult();
}
