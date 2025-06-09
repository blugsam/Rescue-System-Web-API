using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RescueSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HealthMetricNowIsAdditional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HealthProfileId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QualityLevel",
                table: "Alerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string[]>(
                name: "ValidationErrors",
                table: "Alerts",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.CreateTable(
                name: "HealthProfileThresholds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HighPulseThreshold = table.Column<double>(type: "double precision", nullable: true),
                    LowPulseThreshold = table.Column<double>(type: "double precision", nullable: true),
                    HighTempThreshold = table.Column<double>(type: "double precision", nullable: true),
                    LowTempThreshold = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthProfileThresholds", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "HealthProfileThresholds",
                columns: new[] { "Id", "HighPulseThreshold", "HighTempThreshold", "LowPulseThreshold", "LowTempThreshold", "ProfileName" },
                values: new object[,]
                {
                    { new Guid("4d74cc16-2a76-4772-9643-e82cb122c898"), 180.0, 38.5, 40.0, 35.0, "Athlete" },
                    { new Guid("a66ca370-bf93-4c12-be9a-086b98136eea"), 160.0, 38.5, 50.0, 35.0, "Default" },
                    { new Guid("c1464740-bca9-4289-8415-2e8eadf6d623"), 140.0, 38.5, 55.0, 35.0, "Hypertensive" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_HealthProfileId",
                table: "Users",
                column: "HealthProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthProfileThresholds_ProfileName",
                table: "HealthProfileThresholds",
                column: "ProfileName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_HealthProfileThresholds_HealthProfileId",
                table: "Users",
                column: "HealthProfileId",
                principalTable: "HealthProfileThresholds",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_HealthProfileThresholds_HealthProfileId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "HealthProfileThresholds");

            migrationBuilder.DropIndex(
                name: "IX_Users_HealthProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HealthProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QualityLevel",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "ValidationErrors",
                table: "Alerts");
        }
    }
}
