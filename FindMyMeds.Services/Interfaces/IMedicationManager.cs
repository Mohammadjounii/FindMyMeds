using FindMyMeds.Core.DTOs;
using FindMyMeds.Core.Entities;

namespace FindMyMeds.Services.Interfaces
{
    public interface IMedicationManager
    {
        Task AddAsync(Medication medication);
        Task<List<MedicationAvailabilityDto>> SearchAsync(string query);
        Task<MedicationDetailsDto?> GetDetailsAsync(int id);
        Task<List<MedicationSearchDto>> GetAllAsync();

        Task<List<MedicationSearchDto>> GetAllForDropdownAsync();
        Task<List<MedicationSearchDto>> SearchForDropdownAsync(string term);
        Task<List<MedicationSubstituteDto>> GetAvailableSubstitutesInPharmacyAsync(int medicationId, int pharmacyId);

    }
}
