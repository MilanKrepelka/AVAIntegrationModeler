using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.ScenarioAggregate;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

/// <summary>
/// Konfigurace pro entitu Scenario.
/// </summary>
public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
  public void Configure(EntityTypeBuilder<Scenario> builder)
  {
    // Kód scénáře - povinný, unikátní, max. délka
    builder.Property(p => p.Code)
        .HasMaxLength(100)
        .IsRequired();

    // Lokalizovaný název (value object)
    builder.OwnsOne(p => p.Name);

    // Lokalizovaný popis (value object)
    builder.OwnsOne(p => p.Description);
    /*
    // Vstupní feature - pouze Id
    builder.Property(p => p.InputFeature)
        .HasConversion(
            v => v != null ? Ind : null,
            v => v != null ? new FeatureId(v) : null);

    // Výstupní feature - pouze Id
    builder.Property(p => p.OutputFeature)
        .HasConversion(
            v => v != null ? v.Id : null,
            v => v != null ? new FeatureId(v) : null);
    */
  }
}
