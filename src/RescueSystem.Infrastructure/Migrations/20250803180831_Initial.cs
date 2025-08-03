using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RescueSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthProfileThresholdss",
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
                    table.PrimaryKey("PK_HealthProfileThresholdss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    MedicalNotes = table.Column<string>(type: "text", nullable: true),
                    EmergencyContact = table.Column<string>(type: "text", nullable: true),
                    HealthProfileId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_HealthProfileThresholdss_HealthProfileId",
                        column: x => x.HealthProfileId,
                        principalTable: "HealthProfileThresholdss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Bracelets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bracelets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bracelets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    BraceletId = table.Column<Guid>(type: "uuid", nullable: true),
                    QualityLevel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Bracelets_BraceletId",
                        column: x => x.BraceletId,
                        principalTable: "Bracelets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertTriggers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    AlertId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertTriggers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertTriggers_Alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "HealthMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Pulse = table.Column<double>(type: "double precision", nullable: true),
                    BodyTemperature = table.Column<double>(type: "double precision", nullable: true),
                    RawDataJson = table.Column<string>(type: "text", nullable: true),
                    AlertId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthMetrics", x => x.Id);
                    table.CheckConstraint("CK_HealthMetric_Pulse", "\"Pulse\" IS NULL OR (\"Pulse\" >= 30 AND \"Pulse\" <= 250)");
                    table.CheckConstraint("CK_HealthMetric_Temp", "\"BodyTemperature\" IS NULL OR (\"BodyTemperature\" >= 30 AND \"BodyTemperature\" <= 45)");
                    table.ForeignKey(
                        name: "FK_HealthMetrics_Alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "Alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "HealthProfileThresholdss",
                columns: new[] { "Id", "HighPulseThreshold", "HighTempThreshold", "LowPulseThreshold", "LowTempThreshold", "ProfileName" },
                values: new object[,]
                {
                    { new Guid("4d74cc16-2a76-4772-9643-e82cb122c898"), 180.0, 38.5, 40.0, 35.0, "Athlete" },
                    { new Guid("a66ca370-bf93-4c12-be9a-086b98136eea"), 160.0, 38.5, 50.0, 35.0, "Default" },
                    { new Guid("c1464740-bca9-4289-8415-2e8eadf6d623"), 140.0, 38.5, 55.0, 35.0, "Hypertensive" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_BraceletId",
                table: "Alerts",
                column: "BraceletId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertTriggers_AlertId_Type",
                table: "AlertTriggers",
                columns: new[] { "AlertId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlertValidationErrors_AlertId",
                table: "AlertValidationErrors",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_Bracelets_SerialNumber",
                table: "Bracelets",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bracelets_UserId",
                table: "Bracelets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HealthMetrics_AlertId",
                table: "HealthMetrics",
                column: "AlertId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HealthProfileThresholdss_ProfileName",
                table: "HealthProfileThresholdss",
                column: "ProfileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_HealthProfileId",
                table: "Users",
                column: "HealthProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertTriggers");

            migrationBuilder.DropTable(
                name: "AlertValidationErrors");

            migrationBuilder.DropTable(
                name: "HealthMetrics");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "Bracelets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "HealthProfileThresholdss");
        }
    }
}
