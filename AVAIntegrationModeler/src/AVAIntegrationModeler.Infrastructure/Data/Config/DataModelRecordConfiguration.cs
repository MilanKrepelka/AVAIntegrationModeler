using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AVAIntegrationModeler.Infrastructure.Data.Config;

public class DataModelRecordConfiguration : IEntityTypeConfiguration<DataModelRecord>
{
  public void Configure(EntityTypeBuilder<DataModelRecord> builder)
  {
    builder.ToTable("DataModelRecords");
    builder.HasKey(e => e.Id);

    builder.Property(e => e.ModelId)
      .IsRequired();

    builder.Property(e => e.ExternalId)
      .HasMaxLength(500);

    builder.HasMany<DataModelRecordField>(e => e.Fields)
      .WithOne()
      .HasForeignKey("DataModelRecordId")
      .OnDelete(DeleteBehavior.Cascade);

    builder.Navigation(e => e.Fields)
      .UsePropertyAccessMode(PropertyAccessMode.Field)
      .AutoInclude();
  }
}
