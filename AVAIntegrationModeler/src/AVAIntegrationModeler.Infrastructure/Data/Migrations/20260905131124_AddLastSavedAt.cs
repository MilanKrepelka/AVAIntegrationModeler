using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLastSavedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "Scenarios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "IntegrationMaps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "IntegrationMapItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "IntegrationMapActivationKeys",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "Features",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "FeatureIncludedModels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "FeatureIncludedFeatures",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "Deployments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "DeploymentDataModels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "DataModels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "DataModelRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "DataModelRecordFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "DataModelFields",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "DataModelFieldEntityTypeReferences",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "Contributors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "Areas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "IntegrationMaps");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "IntegrationMapItems");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "IntegrationMapActivationKeys");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "FeatureIncludedModels");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "FeatureIncludedFeatures");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "Deployments");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "DeploymentDataModels");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "DataModels");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "DataModelRecords");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "DataModelRecordFields");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "DataModelFields");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "DataModelFieldEntityTypeReferences");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "Contributors");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "Areas");
        }
    }
}
