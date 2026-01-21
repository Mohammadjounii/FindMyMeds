using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyMeds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatenotif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicationId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicationId",
                table: "Notifications");
        }
    }
}
