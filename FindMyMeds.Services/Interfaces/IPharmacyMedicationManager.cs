using FindMyMeds.Core.Entities;

namespace FindMyMeds.Services.Interfaces
{
    public interface IPharmacyMedicationManager
    {
        Task<List<PharmacyMedication>> GetByPharmacyIdAsync(int pharmacyId);

        Task<PharmacyMedication?> GetAsync(int id);

        Task AddAsync(PharmacyMedication pharmacyMedication);

        Task UpdateAsync(PharmacyMedication pharmacyMedication);

        Task DeleteAsync(int id);

        Task<PharmacyMedication?> GetByPharmacyAndMedicationAsync(int pharmacyId, int medicationId);
    }
}
