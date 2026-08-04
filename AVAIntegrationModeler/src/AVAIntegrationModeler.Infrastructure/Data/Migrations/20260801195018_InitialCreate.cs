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
                name: "Contributors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PhoneNumber_CountryCode = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber_Number = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber_Extension = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributors", x => x.Id);
                });

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
                name: "Deployments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Ticket = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name_CZ = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Name_EN = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description_CZ = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Description_EN = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

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
                name: "Scenarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name_CzechValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Name_EnglishValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description_CzechValue = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Description_EnglishValue = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    InputFeature = table.Column<Guid>(type: "TEXT", nullable: true),
                    OutputFeature = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenarios", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "FeatureIncludedFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IncludedFeatureId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsumeOnly = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IncludedModelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReadOnly = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
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
                name: "IX_DataModelRecordFields_DataModelRecordId",
                table: "DataModelRecordFields",
                column: "DataModelRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_DataModels_AreaId",
                table: "DataModels",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentDataModels_DeploymentId",
                table: "DeploymentDataModels",
                column: "DeploymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Deployments_Code",
                table: "Deployments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureIncludedFeatures_IncludedFeatureId",
                table: "FeatureIncludedFeatures",
                column: "IncludedFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureIncludedFeatures_OwnerFeatureId",
                table: "FeatureIncludedFeatures",
                column: "OwnerFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureIncludedModels_IncludedModelId",
                table: "FeatureIncludedModels",
                column: "IncludedModelId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureIncludedModels_OwnerFeatureId",
                table: "FeatureIncludedModels",
                column: "OwnerFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_Code",
                table: "Features",
                column: "Code",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_Code",
                table: "Scenarios",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contributors");

            migrationBuilder.DropTable(
                name: "DataModelFieldEntityTypeReferences");

            migrationBuilder.DropTable(
                name: "DataModelRecordFields");

            migrationBuilder.DropTable(
                name: "DeploymentDataModels");

            migrationBuilder.DropTable(
                name: "FeatureIncludedFeatures");

            migrationBuilder.DropTable(
                name: "FeatureIncludedModels");

            migrationBuilder.DropTable(
                name: "IntegrationMapActivationKeys");

            migrationBuilder.DropTable(
                name: "Scenarios");

            migrationBuilder.DropTable(
                name: "DataModelFields");

            migrationBuilder.DropTable(
                name: "DataModelRecords");

            migrationBuilder.DropTable(
                name: "Deployments");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "IntegrationMapItems");

            migrationBuilder.DropTable(
                name: "DataModels");

            migrationBuilder.DropTable(
                name: "IntegrationMaps");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
