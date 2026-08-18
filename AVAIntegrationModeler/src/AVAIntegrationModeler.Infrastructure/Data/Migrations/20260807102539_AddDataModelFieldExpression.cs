using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDataModelFieldExpression : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpressionOrder",
                table: "DataModelFields",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpressionValue",
                table: "DataModelFields",
                type: "TEXT",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpressionOrder",
                table: "DataModelFields");

            migrationBuilder.DropColumn(
                name: "ExpressionValue",
                table: "DataModelFields");
        }
    }
}
