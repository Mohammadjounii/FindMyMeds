using FindMyMeds.Core.Entities;

namespace FindMyMeds.Services.Interfaces
{
    public interface IPharmacyManager
    {
        Task<List<Pharmacy>> GetAllAsync();
        Task<Pharmacy?> GetByIdAsync(int id);
        Task AddAsync(Pharmacy pharmacy);
        Task UpdateAsync(Pharmacy pharmacy);
        Task DeleteAsync(int id);
        Task ApproveAsync(int id);

    }
}
