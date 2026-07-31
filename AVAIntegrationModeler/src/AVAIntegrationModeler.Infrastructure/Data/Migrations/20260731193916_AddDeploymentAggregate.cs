using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeploymentAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deployments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeploymentDataModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeploymentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataModelId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentDataModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeploymentDataModels_Deployments_DeploymentId",
                        column: x => x.DeploymentId,
                        principalTable: "Deployments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentDataModels_DeploymentId",
                table: "DeploymentDataModels",
                column: "DeploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Deployments_Code",
                table: "Deployments",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeploymentDataModels");

            migrationBuilder.DropTable(
                name: "Deployments");
        }
    }
}
