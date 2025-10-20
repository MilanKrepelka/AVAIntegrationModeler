using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations;

  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.CreateTable(
              name: "Scenarios",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "TEXT", nullable: false),
                  Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                  Name_CzechValue = table.Column<string>(type: "TEXT", nullable: false),
                  Name_EnglishValue = table.Column<string>(type: "TEXT", nullable: false),
                  Description_CzechValue = table.Column<string>(type: "TEXT", nullable: false),
                  Description_EnglishValue = table.Column<string>(type: "TEXT", nullable: false),
                  InputFeature = table.Column<Guid>(type: "TEXT", nullable: true),
                  OutputFeature = table.Column<Guid>(type: "TEXT", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Scenarios", x => x.Id);
              });
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropTable(
              name: "Scenarios");
      }
  }
