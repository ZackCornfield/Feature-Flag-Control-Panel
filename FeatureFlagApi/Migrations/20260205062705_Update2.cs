using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureFlagApi.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeatureFlags_Key",
                table: "FeatureFlags");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureFlags_Key_Environment",
                table: "FeatureFlags",
                columns: new[] { "Key", "Environment" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeatureFlags_Key_Environment",
                table: "FeatureFlags");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureFlags_Key",
                table: "FeatureFlags",
                column: "Key",
                unique: true);
        }
    }
}
