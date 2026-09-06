using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMapLayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MapLayouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DiagramJson = table.Column<string>(type: "TEXT", nullable: true),
                    LastMapSave = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastSavedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapLayouts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapLayouts_Key",
                table: "MapLayouts",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapLayouts");
        }
    }
}
