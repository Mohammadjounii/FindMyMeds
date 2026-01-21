using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyMeds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAvailabilityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailabilityLogs");

            migrationBuilder.DropTable(
                name: "PharmacyStaff");

            migrationBuilder.DropTable(
                name: "UserSearchLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvailabilityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacyMedicationId = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NewQuantity = table.Column<int>(type: "int", nullable: false),
                    OldQuantity = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailabilityLogs_PharmacyMedications_PharmacyMedicationId",
                        column: x => x.PharmacyMedicationId,
                        principalTable: "PharmacyMedications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PharmacyStaff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PharmacyId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacyStaff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PharmacyStaff_Pharmacies_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSearchLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicationId = table.Column<int>(type: "int", nullable: true),
                    QueryText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SearchedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSearchLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityLogs_PharmacyMedicationId",
                table: "AvailabilityLogs",
                column: "PharmacyMedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyStaff_PharmacyId",
                table: "PharmacyStaff",
                column: "PharmacyId");
        }
    }
}
