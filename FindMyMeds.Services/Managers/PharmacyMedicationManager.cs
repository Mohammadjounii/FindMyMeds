using FindMyMeds.Core.Entities;
using FindMyMeds.Infrastructure.Data;
using FindMyMeds.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FindMyMeds.Services.Managers
{
    public class PharmacyMedicationManager : IPharmacyMedicationManager
    {
        private readonly ApplicationDbContext _context;
        private readonly IMedicationNotifier _notifier;

        public PharmacyMedicationManager(
            ApplicationDbContext context,
            IMedicationNotifier notifier)
        {
            _context = context;
            _notifier = notifier;
        }

        public async Task<List<PharmacyMedication>> GetByPharmacyIdAsync(int pharmacyId)
        {
            return await _context.PharmacyMedications
                .AsNoTracking()
                .Include(pm => pm.Medication)
                .Where(pm => pm.PharmacyId == pharmacyId)
                .OrderBy(pm => pm.Medication!.Name)
                .ToListAsync();
        }

        public async Task<PharmacyMedication?> GetAsync(int id)
        {
            return await _context.PharmacyMedications
                .Include(pm => pm.Medication)
                .FirstOrDefaultAsync(pm => pm.Id == id);
        }

        public async Task AddAsync(PharmacyMedication entity)
        {
            await _context.PharmacyMedications.AddAsync(entity);
            await _context.SaveChangesAsync();

            await _notifier.MedicationUpdatedAsync(entity.MedicationId);
        }

        public async Task UpdateAsync(PharmacyMedication entity)
        {
            var tracked = _context.ChangeTracker.Entries<PharmacyMedication>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (tracked != null)
            {
                tracked.CurrentValues.SetValues(entity);
                tracked.Entity.LastUpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var existing = await _context.PharmacyMedications.FindAsync(entity.Id);
                if (existing != null)
                {
                    existing.MedicationId = entity.MedicationId;
                    existing.Quantity = entity.Quantity;
                    existing.ExpiryDate = entity.ExpiryDate;
                    existing.Price = entity.Price;
                    existing.LastUpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            await _notifier.MedicationUpdatedAsync(entity.MedicationId);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.PharmacyMedications.FindAsync(id);
            if (item != null)
            {
                _context.PharmacyMedications.Remove(item);
                await _context.SaveChangesAsync();

                await _notifier.MedicationUpdatedAsync(item.MedicationId);
            }
        }

        public async Task<PharmacyMedication?> GetByPharmacyAndMedicationAsync(int pharmacyId, int medicationId)
        {
            return await _context.PharmacyMedications
                .FirstOrDefaultAsync(pm =>
                    pm.PharmacyId == pharmacyId &&
                    pm.MedicationId == medicationId);
        }
    }
}
