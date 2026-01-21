using FindMyMeds.Core.Entities;
using FindMyMeds.Core.Interfaces;
using FindMyMeds.Services.Interfaces;

namespace FindMyMeds.Services.Managers
{
    public class PharmacyManager : IPharmacyManager
    {
        private readonly IRepository<Pharmacy> _repo;

        public PharmacyManager(IRepository<Pharmacy> repo)
        {
            _repo = repo;
        }

        public async Task AddAsync(Pharmacy pharmacy)
        {
            await _repo.AddAsync(pharmacy);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
            {
                _repo.Remove(entity);
                await _repo.SaveChangesAsync();
            }
        }

        public Task<List<Pharmacy>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<Pharmacy?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public async Task UpdateAsync(Pharmacy pharmacy)
        {
            _repo.Update(pharmacy);
            await _repo.SaveChangesAsync();
        }

        public async Task ApproveAsync(int id)
        {
            var ph = await _repo.GetByIdAsync(id);
            if (ph == null) return;

            ph.IsApproved = true;
            _repo.Update(ph);
            await _repo.SaveChangesAsync();
        }


    }
}
