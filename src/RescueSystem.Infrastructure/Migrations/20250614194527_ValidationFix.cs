using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RescueSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ValidationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RawDataJson",
                table: "HealthMetrics",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawDataJson",
                table: "HealthMetrics");
        }
    }
}
