using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureFlagApi.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isEnabled",
                table: "FeatureFlagOverrides",
                newName: "IsEnabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "FeatureFlagOverrides",
                newName: "isEnabled");
        }
    }
}
