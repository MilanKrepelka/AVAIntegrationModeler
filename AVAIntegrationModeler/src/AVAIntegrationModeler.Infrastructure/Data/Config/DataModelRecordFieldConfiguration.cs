using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

public class DataModelRecordFieldConfiguration : IEntityTypeConfiguration<DataModelRecordField>
{
  public void Configure(EntityTypeBuilder<DataModelRecordField> builder)
  {
    builder.ToTable("DataModelRecordFields");
    builder.HasKey(e => e.Id);

    builder.Property<Guid>("DataModelRecordId").IsRequired();

    builder.Property(e => e.Key)
      .IsRequired()
      .HasMaxLength(200);

    builder.Property(e => e.IsLocalized)
      .IsRequired()
      .HasDefaultValue(false);

    builder.Property(e => e.StringValue)
      .HasMaxLength(2000);

    builder.Property(e => e.CzechValue)
      .HasMaxLength(2000);

    builder.Property(e => e.EnglishValue)
      .HasMaxLength(2000);
  }
}
