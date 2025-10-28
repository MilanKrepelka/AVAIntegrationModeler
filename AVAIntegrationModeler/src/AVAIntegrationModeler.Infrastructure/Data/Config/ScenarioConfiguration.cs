using AVAIntegrationModeler.Domain.ScenarioAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

/// <summary>
/// Konfigurace pro entitu Scenario.
/// </summary>
public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
  public void Configure(EntityTypeBuilder<Scenario> builder)
  {
    // Primární klíč
    builder.HasKey(p => p.Id);

    // Kód scénáře - povinný, unikátní, max. délka
    builder.Property(p => p.Code)
        .HasMaxLength(100)
        .IsRequired();

    builder.HasIndex(p => p.Code)
        .IsUnique();

    // Lokalizovaný název (value object)
    builder.OwnsOne(p => p.Name, nameBuilder =>
    {
      nameBuilder.Property(n => n.CzechValue)
          .HasColumnName("Name_CzechValue")
          .HasMaxLength(200);

      nameBuilder.Property(n => n.EnglishValue)
          .HasColumnName("Name_EnglishValue")
          .HasMaxLength(200);
    });

    // Lokalizovaný popis (value object)
    builder.OwnsOne(p => p.Description, descBuilder =>
    {
      descBuilder.Property(d => d.CzechValue)
          .HasColumnName("Description_CzechValue")
          .HasMaxLength(1000);

      descBuilder.Property(d => d.EnglishValue)
          .HasColumnName("Description_EnglishValue")
          .HasMaxLength(1000);
    });

    // Vstupní feature (nullable reference)
    builder.Property(p => p.InputFeature)
        .IsRequired(false);

    // Výstupní feature (nullable reference)
    builder.Property(p => p.OutputFeature)
        .IsRequired(false);

    // Název tabulky
    builder.ToTable("Scenarios");
  }
}
