using FindMyMeds.Core.DTOs.Reports;
using FindMyMeds.Infrastructure.Data;
using FindMyMeds.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FindMyMeds.Services.Managers
{
    public class ReportManager : IReportManager
    {
        private readonly ApplicationDbContext _context;

        public ReportManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MedicationReportDto>> GetMedicationAvailabilityReportAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.PharmacyMedications
                .GroupBy(pm => pm.Medication.Name)
                .Select(g => new MedicationReportDto
                {
                    MedicationName = g.Key,
                    TotalPharmacies = g.Count(),

                    AvailableCount = g.Count(x =>
                        x.Quantity > 0 &&
                        (!x.ExpiryDate.HasValue || x.ExpiryDate.Value.Date >= today)
                    ),

                    OutOfStockCount = g.Count(x =>
                        x.Quantity == 0 ||
                        (x.ExpiryDate.HasValue && x.ExpiryDate.Value.Date < today)
                    )
                })
                .OrderByDescending(x => x.OutOfStockCount)
                .ToListAsync();
        }


        public async Task<List<PharmacyActivityReportDto>> GetPharmacyActivityReportAsync()
        {
            return await _context.PharmacyMedications
                .GroupBy(pm => pm.Pharmacy.Name)
                .Select(g => new PharmacyActivityReportDto
                {
                    PharmacyName = g.Key,
                    UpdatesCount = g.Count(),
                })
                .OrderByDescending(x => x.UpdatesCount)
                .ToListAsync();
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var today = DateTime.UtcNow.Date;

            var availableMedicationIds = await _context.PharmacyMedications
                .Where(x =>
                    x.Quantity > 0 &&
                    (!x.ExpiryDate.HasValue || x.ExpiryDate.Value.Date >= today)
                )
                .Select(x => x.MedicationId)
                .Distinct()
                .ToListAsync();

            return new DashboardStatsDto
            {
                TotalPharmacies = await _context.Pharmacies.CountAsync(),
                TotalMedications = await _context.Medications.CountAsync(),

                AvailableMedications = availableMedicationIds.Count,

                OutOfStockMedications =
                    await _context.Medications.CountAsync()
                    - availableMedicationIds.Count
            };

        }

    }
}
