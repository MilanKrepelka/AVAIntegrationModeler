using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.RenameColumn(
              name: "ReadOnly",
              table: "FeatureIncludedFeatures",
              newName: "ConsumeOnly");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.RenameColumn(
              name: "ConsumeOnly",
              table: "FeatureIncludedFeatures",
              newName: "ReadOnly");
      }
  }
