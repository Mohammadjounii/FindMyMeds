using FindMyMeds.Core.Entities;
using FindMyMeds.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FindMyMeds.Infrastructure.Seed
{
    public static class MedicationSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Medications.AnyAsync())
                return;

            var medications = new List<Medication>
            {
                new Medication { Name = "Paracetamol", Description = "Pain reliever and fever reducer" },
                new Medication { Name = "Ibuprofen", Description = "Nonsteroidal anti-inflammatory drug" },
                new Medication { Name = "Amoxicillin", Description = "Antibiotic used to treat bacterial infections" }
            };

            await context.Medications.AddRangeAsync(medications);
            await context.SaveChangesAsync();
        }
    }
}
