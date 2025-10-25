using AVAIntegrationModeler.Domain.AreaAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

/// <summary>
/// EF Core konfigurace pro agregát Area.
/// </summary>
public class AreaConfiguration : IEntityTypeConfiguration<Area>
{
  public void Configure(EntityTypeBuilder<Area> builder)
  {
    builder.ToTable("Areas");
    builder.HasKey(e => e.Id);
    
    builder.Property(e => e.Code)
      .IsRequired()
      .HasMaxLength(100);
    
    builder.Property(e => e.Name)
      .IsRequired()
      .HasMaxLength(200);
    
    // Index pro rychlé vyhledávání podle kódu
    builder.HasIndex(e => e.Code)
      .IsUnique();
  }
}
