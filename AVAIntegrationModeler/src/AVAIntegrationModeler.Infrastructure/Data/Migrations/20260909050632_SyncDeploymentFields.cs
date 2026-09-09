using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVAIntegrationModeler.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// Prázdná migrace — sloupce LastDeploymentDateTime a LastSaveDateTime přidává migrace AddDeploymentDateTimes.
    /// Snapshot byl v rámci merge větví synchronizován, tato migrace zajišťuje konzistenci záznamu v __EFMigrationsHistory.
    /// </remarks>
    public partial class SyncDeploymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
