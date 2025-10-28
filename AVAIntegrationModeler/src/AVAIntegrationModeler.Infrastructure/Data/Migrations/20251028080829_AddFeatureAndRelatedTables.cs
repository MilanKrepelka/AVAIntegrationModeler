using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations;

  /// <inheritdoc />
  public partial class AddFeatureAndRelatedTables : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.CreateTable(
              name: "Areas",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "TEXT", nullable: false),
                  Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                  Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Areas", x => x.Id);
              });

          migrationBuilder.CreateTable(
              name: "Features",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "TEXT", nullable: false),
                  Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                  Name_CZ = table.Column<string>(type: "TEXT", nullable: false),
                  Name_EN = table.Column<string>(type: "TEXT", nullable: false),
                  Description_CZ = table.Column<string>(type: "TEXT", nullable: false),
                  Description_EN = table.Column<string>(type: "TEXT", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Features", x => x.Id);
              });

          migrationBuilder.CreateTable(
              name: "DataModels",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "TEXT", nullable: false),
                  Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                  Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                  Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                  Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                  IsAggregateRoot = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                  AreaId = table.Column<Guid>(type: "TEXT", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_DataModels", x => x.Id);
                  table.ForeignKey(
                      name: "FK_DataModels_Areas_AreaId",
                      column: x => x.AreaId,
                      principalTable: "Areas",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.SetNull);
              });

          migrationBuilder.CreateTable(
              name: "FeatureIncludedFeatures",
              columns: table => new
              {
                  Id = table.Column<int>(type: "INTEGER", nullable: false)
                      .Annotation("Sqlite:Autoincrement", true),
                  IncludedFeatureId = table.Column<Guid>(type: "TEXT", nullable: false),
                  ConsumeOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                  OwnerFeatureId = table.Column<Guid>(type: "TEXT", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_FeatureIncludedFeatures", x => x.Id);
                  table.ForeignKey(
                      name: "FK_FeatureIncludedFeatures_Features_OwnerFeatureId",
                      column: x => x.OwnerFeatureId,
                      principalTable: "Features",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "FeatureIncludedModels",
              columns: table => new
              {
                  Id = table.Column<int>(type: "INTEGER", nullable: false)
                      .Annotation("Sqlite:Autoincrement", true),
                  ConsumeOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                  IncludedModelId = table.Column<Guid>(type: "TEXT", nullable: false),
                  OwnerFeatureId = table.Column<Guid>(type: "TEXT", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_FeatureIncludedModels", x => x.Id);
                  table.ForeignKey(
                      name: "FK_FeatureIncludedModels_Features_OwnerFeatureId",
                      column: x => x.OwnerFeatureId,
                      principalTable: "Features",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "DataModelFields",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "TEXT", nullable: false),
                  Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                  Label = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                  Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                  IsPublishedForLookup = table.Column<bool>(type: "INTEGER", nullable: false),
                  IsCollection = table.Column<bool>(type: "INTEGER", nullable: false),
                  IsLocalized = table.Column<bool>(type: "INTEGER", nullable: false),
                  IsNullable = table.Column<bool>(type: "INTEGER", nullable: false),
                  FieldType = table.Column<int>(type: "INTEGER", nullable: false),
                  DataModelId = table.Column<Guid>(type: "TEXT", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_DataModelFields", x => x.Id);
                  table.ForeignKey(
                      name: "FK_DataModelFields_DataModels_DataModelId",
                      column: x => x.DataModelId,
                      principalTable: "DataModels",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "DataModelFieldEntityTypeReferences",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "TEXT", nullable: false),
                  DataModelFieldId = table.Column<Guid>(type: "TEXT", nullable: false),
                  ReferencedEntityTypeId = table.Column<Guid>(type: "TEXT", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_DataModelFieldEntityTypeReferences", x => x.Id);
                  table.ForeignKey(
                      name: "FK_DataModelFieldEntityTypeReferences_DataModelFields_DataModelFieldId",
                      column: x => x.DataModelFieldId,
                      principalTable: "DataModelFields",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateIndex(
              name: "IX_Areas_Code",
              table: "Areas",
              column: "Code",
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_DataModelFieldEntityTypeReferences_DataModelFieldId",
              table: "DataModelFieldEntityTypeReferences",
              column: "DataModelFieldId");

          migrationBuilder.CreateIndex(
              name: "IX_DataModelFieldEntityTypeReferences_DataModelFieldId_ReferencedEntityTypeId",
              table: "DataModelFieldEntityTypeReferences",
              columns: new[] { "DataModelFieldId", "ReferencedEntityTypeId" },
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_DataModelFieldEntityTypeReferences_ReferencedEntityTypeId",
              table: "DataModelFieldEntityTypeReferences",
              column: "ReferencedEntityTypeId");

          migrationBuilder.CreateIndex(
              name: "IX_DataModelFields_DataModelId",
              table: "DataModelFields",
              column: "DataModelId");

          migrationBuilder.CreateIndex(
              name: "IX_DataModels_AreaId",
              table: "DataModels",
              column: "AreaId");

          migrationBuilder.CreateIndex(
              name: "IX_FeatureIncludedFeatures_OwnerFeatureId",
              table: "FeatureIncludedFeatures",
              column: "OwnerFeatureId");

          migrationBuilder.CreateIndex(
              name: "IX_FeatureIncludedModels_OwnerFeatureId",
              table: "FeatureIncludedModels",
              column: "OwnerFeatureId");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropTable(
              name: "DataModelFieldEntityTypeReferences");

          migrationBuilder.DropTable(
              name: "FeatureIncludedFeatures");

          migrationBuilder.DropTable(
              name: "FeatureIncludedModels");

          migrationBuilder.DropTable(
              name: "DataModelFields");

          migrationBuilder.DropTable(
              name: "Features");

          migrationBuilder.DropTable(
              name: "DataModels");

          migrationBuilder.DropTable(
              name: "Areas");
      }
  }
