using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RescueSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDbContextConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_HealthProfileThresholds_HealthProfileId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HealthProfileThresholds",
                table: "HealthProfileThresholds");

            migrationBuilder.RenameTable(
                name: "HealthProfileThresholds",
                newName: "HealthProfileThresholdss");

            migrationBuilder.RenameIndex(
                name: "IX_HealthProfileThresholds_ProfileName",
                table: "HealthProfileThresholdss",
                newName: "IX_HealthProfileThresholdss_ProfileName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HealthProfileThresholdss",
                table: "HealthProfileThresholdss",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_HealthProfileThresholdss_HealthProfileId",
                table: "Users",
                column: "HealthProfileId",
                principalTable: "HealthProfileThresholdss",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_HealthProfileThresholdss_HealthProfileId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HealthProfileThresholdss",
                table: "HealthProfileThresholdss");

            migrationBuilder.RenameTable(
                name: "HealthProfileThresholdss",
                newName: "HealthProfileThresholds");

            migrationBuilder.RenameIndex(
                name: "IX_HealthProfileThresholdss_ProfileName",
                table: "HealthProfileThresholds",
                newName: "IX_HealthProfileThresholds_ProfileName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HealthProfileThresholds",
                table: "HealthProfileThresholds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_HealthProfileThresholds_HealthProfileId",
                table: "Users",
                column: "HealthProfileId",
                principalTable: "HealthProfileThresholds",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
