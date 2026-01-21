using FindMyMeds.Core.Entities;
using FindMyMeds.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FindMyMeds.Infrastructure.Repositories
{
    public class PharmacyRepository : Repository<Pharmacy>
    {
        public PharmacyRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<Pharmacy>> GetAllAsync()
        {
            return await _context.Pharmacies
                .Include(p => p.OwnerUser)           
                .ToListAsync();
        }
        public override async Task<Pharmacy?> GetByIdAsync(int id)
        {
            return await _context.Pharmacies
                .Include(p => p.OwnerUser)   
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
