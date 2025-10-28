using System.Reflection;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.FeatureAggregate;
using AVAIntegrationModeler.Domain.IntegrationMapAggregate;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Infrastructure.Data.Config;
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
  public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher? dispatcher)
  {
    this._options = options;
    this._dispatcher = dispatcher;
  }
  

  public DbSet<Contributor> Contributors => Set<Contributor>();
  public DbSet<Scenario> Scenarios => Set<Scenario>();
  public DbSet<DataModel> DataModels => Set<DataModel>();
  public DbSet<Area> Areas => Set<Area>();

  public DbSet<DataModelField> DataModelFields => Set<DataModelField>(); // ✅
  public DbSet<DataModelFieldEntityTypeReference> DataModelFieldEntityTypeReferences => Set<DataModelFieldEntityTypeReference>(); // ✅ NOVÉ
  public DbSet<Feature> Features => Set<Feature>(); // ✅ PŘIDÁNO

  /// <summary>
  /// Integration maps - mapování integračních scénářů k oblastem.
  /// </summary>
  public DbSet<IntegrationsMap> IntegrationMaps => Set<IntegrationsMap>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // ✅ Ignorovat SmartEnum typy jako entity
    modelBuilder.Ignore<ContributorStatus>();

    // ✅ Aplikovat všechny konfigurace najednou z aktuálního sestavení
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
