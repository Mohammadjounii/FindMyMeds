using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FindMyMeds.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMedicationSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "Id",
                keyValue: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Medications",
                columns: new[] { "Id", "BrandName", "Description", "MedicationCategoryId", "Name" },
                values: new object[,]
                {
                    { 1, "GSK", "Pain relief tablets", null, "Panadol" },
                    { 2, "Abbott", "Ibuprofen pain relief", null, "Brufen" },
                    { 3, "GSK", "Antibiotic for infections", null, "Augmentin" },
                    { 4, "Bayer", "Reduces pain and fever", null, "Aspirin" }
                });
        }
    }
}
