using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyMeds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePharmacyStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyStaffs_Pharmacies_PharmacyId",
                table: "PharmacyStaffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmacyStaffs",
                table: "PharmacyStaffs");

            migrationBuilder.RenameTable(
                name: "PharmacyStaffs",
                newName: "PharmacyStaff");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacyStaffs_PharmacyId",
                table: "PharmacyStaff",
                newName: "IX_PharmacyStaff_PharmacyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmacyStaff",
                table: "PharmacyStaff",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyStaff_Pharmacies_PharmacyId",
                table: "PharmacyStaff",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyStaff_Pharmacies_PharmacyId",
                table: "PharmacyStaff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmacyStaff",
                table: "PharmacyStaff");

            migrationBuilder.RenameTable(
                name: "PharmacyStaff",
                newName: "PharmacyStaffs");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacyStaff_PharmacyId",
                table: "PharmacyStaffs",
                newName: "IX_PharmacyStaffs_PharmacyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmacyStaffs",
                table: "PharmacyStaffs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyStaffs_Pharmacies_PharmacyId",
                table: "PharmacyStaffs",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
