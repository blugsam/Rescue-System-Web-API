using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RescueSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BigOneMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidationErrors",
                table: "Alerts");

            migrationBuilder.AlterColumn<string>(
                name: "QualityLevel",
                table: "Alerts",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "AlertValidationErrors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: false),
                    AlertId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertValidationErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertValidationErrors_Alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertValidationErrors_AlertId",
                table: "AlertValidationErrors",
                column: "AlertId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertValidationErrors");

            migrationBuilder.AlterColumn<int>(
                name: "QualityLevel",
                table: "Alerts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string[]>(
                name: "ValidationErrors",
                table: "Alerts",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }
    }
}
