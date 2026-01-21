using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyMeds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "Pharmacies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "Pharmacies");
        }
    }
}
