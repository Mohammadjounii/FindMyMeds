using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyMeds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMedicationCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicationCategories_MedicationCategoryId",
                table: "Medications");

            migrationBuilder.DropTable(
                name: "MedicationCategories");

            migrationBuilder.DropIndex(
                name: "IX_Medications_MedicationCategoryId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "MedicationCategoryId",
                table: "Medications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicationCategoryId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicationCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medications_MedicationCategoryId",
                table: "Medications",
                column: "MedicationCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicationCategories_MedicationCategoryId",
                table: "Medications",
                column: "MedicationCategoryId",
                principalTable: "MedicationCategories",
                principalColumn: "Id");
        }
    }
}
