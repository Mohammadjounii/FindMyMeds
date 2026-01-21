using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyMeds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePharmacyAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Pharmacies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Pharmacies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
