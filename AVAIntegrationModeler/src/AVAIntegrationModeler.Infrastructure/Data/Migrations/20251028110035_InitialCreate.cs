using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AreaId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationMapItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScenarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IntegrationsMapId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationMapItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntegrationMapItems_IntegrationMaps_IntegrationsMapId",
                        column: x => x.IntegrationsMapId,
                        principalTable: "IntegrationMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IntegrationMapActivationKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IntegrationMapItemId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationMapActivationKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntegrationMapActivationKeys_IntegrationMapItems_IntegrationMapItemId",
                        column: x => x.IntegrationMapItemId,
                        principalTable: "IntegrationMapItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationMapActivationKeys_IntegrationMapItemId",
                table: "IntegrationMapActivationKeys",
                column: "IntegrationMapItemId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationMapActivationKeys_Key",
                table: "IntegrationMapActivationKeys",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationMapItems_IntegrationsMapId",
                table: "IntegrationMapItems",
                column: "IntegrationsMapId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationMapItems_ScenarioId",
                table: "IntegrationMapItems",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationMaps_AreaId",
                table: "IntegrationMaps",
                column: "AreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntegrationMapActivationKeys");

            migrationBuilder.DropTable(
                name: "IntegrationMapItems");

            migrationBuilder.DropTable(
                name: "IntegrationMaps");
        }
    }
}
