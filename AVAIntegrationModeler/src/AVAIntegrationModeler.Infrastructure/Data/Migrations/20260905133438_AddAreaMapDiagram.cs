using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaMapDiagram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastMapSave",
                table: "Areas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapDiagramJson",
                table: "Areas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMapSave",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "MapDiagramJson",
                table: "Areas");
        }
    }
}
