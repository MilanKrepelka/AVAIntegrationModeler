using AVAIntegrationModeler.Domain.MapLayoutAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

/// <summary>
/// EF Core konfigurace pro entitu MapLayout.
/// </summary>
public class MapLayoutConfiguration : IEntityTypeConfiguration<MapLayout>
{
  public void Configure(EntityTypeBuilder<MapLayout> builder)
  {
    builder.ToTable("MapLayouts");
    builder.HasKey(e => e.Id);
    builder.Property(e => e.Key).IsRequired().HasMaxLength(100);
    builder.HasIndex(e => e.Key).IsUnique();
  }
}
