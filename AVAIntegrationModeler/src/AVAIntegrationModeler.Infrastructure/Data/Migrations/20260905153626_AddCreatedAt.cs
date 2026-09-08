using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Scenarios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IntegrationMaps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IntegrationMapItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IntegrationMapActivationKeys",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Features",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FeatureIncludedModels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FeatureIncludedFeatures",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Deployments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DeploymentDataModels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DataModels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DataModelRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DataModelRecordFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DataModelFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DataModelFieldEntityTypeReferences",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Contributors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Areas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IntegrationMaps");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IntegrationMapItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IntegrationMapActivationKeys");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FeatureIncludedModels");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FeatureIncludedFeatures");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Deployments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DeploymentDataModels");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DataModels");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DataModelRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DataModelRecordFields");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DataModelFields");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DataModelFieldEntityTypeReferences");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Contributors");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Areas");
        }
    }
}
