using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDataModelRecordAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataModelRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataModelRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataModelRecordFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsLocalized = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    StringValue = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CzechValue = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    EnglishValue = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DataModelRecordId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataModelRecordFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataModelRecordFields_DataModelRecords_DataModelRecordId",
                        column: x => x.DataModelRecordId,
                        principalTable: "DataModelRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataModelRecordFields_DataModelRecordId",
                table: "DataModelRecordFields",
                column: "DataModelRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataModelRecordFields");

            migrationBuilder.DropTable(
                name: "DataModelRecords");
        }
    }
}
