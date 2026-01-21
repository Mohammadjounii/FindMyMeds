using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyMeds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWantsToOwnPharmacy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WantsToOwnPharmacy",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WantsToOwnPharmacy",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
