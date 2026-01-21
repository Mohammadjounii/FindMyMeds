using FindMyMeds.Core.DTOs.Reports;

namespace FindMyMeds.Services.Interfaces
{
    public interface IReportManager
    {
        Task<List<MedicationReportDto>> GetMedicationAvailabilityReportAsync();
        Task<List<PharmacyActivityReportDto>> GetPharmacyActivityReportAsync();
        Task<DashboardStatsDto> GetDashboardStatsAsync();

    }
}
