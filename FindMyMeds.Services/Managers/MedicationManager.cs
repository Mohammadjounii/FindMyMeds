using FindMyMeds.Core.DTOs;
using FindMyMeds.Core.Entities;
using FindMyMeds.Core.Interfaces;
using FindMyMeds.Infrastructure.Data;
using FindMyMeds.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FindMyMeds.Services.Managers
{
    public class MedicationManager : IMedicationManager
    {
        private readonly IRepository<Medication> _medicationRepo;
        private readonly ApplicationDbContext _context;

        public MedicationManager(
            IRepository<Medication> medicationRepo,
            ApplicationDbContext context)
        {
            _medicationRepo = medicationRepo;
            _context = context;
        }

        public async Task<List<MedicationAvailabilityDto>> SearchAsync(string query)
        {
            query = (query ?? "").Trim().ToLower();

            if (query.Length < 2)
                return new List<MedicationAvailabilityDto>();

            var meds = await _context.Medications
                .AsNoTracking()
                .Where(m => m.Name.ToLower().Contains(query))
                .Select(m => new
                {
                    Medication = m,
                    Score =
                        m.Name.ToLower().StartsWith(query) ? 3 :
                        m.Name.ToLower().Contains(" " + query) ? 2 :
                        1
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Medication.Name)
                .Select(x => x.Medication)
                .Take(30)
                .ToListAsync();

            if (!meds.Any())
                return new List<MedicationAvailabilityDto>();

            var medIds = meds.Select(m => m.Id).ToList();
            var today = DateTime.Today;

            var allStock = await _context.PharmacyMedications
                .AsNoTracking()
                .Include(pm => pm.Pharmacy)
                .Where(pm =>
                    medIds.Contains(pm.MedicationId) &&
                    pm.Pharmacy != null &&
                    pm.Pharmacy.IsApproved &&
                    pm.Quantity > 0 &&
                    pm.Price != null &&
                    (pm.ExpiryDate == null || pm.ExpiryDate >= today)
                )
                .ToListAsync();

            var results = new List<MedicationAvailabilityDto>();

            foreach (var s in allStock)
            {
                var med = meds.First(m => m.Id == s.MedicationId);

                results.Add(new MedicationAvailabilityDto
                {
                    MedicationId = med.Id,
                    MedicationName = med.Name,
                    PharmacyId = s.PharmacyId,
                    PharmacyName = s.Pharmacy!.Name,
                    City = s.Pharmacy.City,
                    Quantity = s.Quantity,
                    Price = s.Price
                });
            }

            return results;
        }
        public async Task<MedicationDetailsDto?> GetDetailsAsync(int id)
        {
            var med = await _medicationRepo.GetByIdAsync(id);
            if (med == null) return null;

            return new MedicationDetailsDto
            {
                Id = med.Id,
                Name = med.Name,
                Description = med.Description
            };
        }

        public async Task<List<MedicationSearchDto>> GetAllAsync()
        {
            var meds = await _medicationRepo.GetAllAsync();
            return meds.Select(m => new MedicationSearchDto
            {
                Id = m.Id,
                Name = m.Name
            }).ToList();
        }

        public async Task<List<MedicationSearchDto>> GetAllForDropdownAsync()
        {
            return await _context.Medications
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .Select(m => new MedicationSearchDto
                {
                    Id = m.Id,
                    Name = m.Name
                })
                .ToListAsync();
        }

        public async Task<List<MedicationSearchDto>> SearchForDropdownAsync(string term)
        {
            term = term?.Trim() ?? "";

            return await _context.Medications
                .AsNoTracking()
                .Where(m => m.Name.Contains(term))
                .OrderBy(m => m.Name)
                .Select(m => new MedicationSearchDto
                {
                    Id = m.Id,
                    Name = m.Name
                })
                .Take(20)
                .ToListAsync();
        }

        public async Task<List<MedicationSubstituteDto>>
     GetAvailableSubstitutesInPharmacyAsync(int medicationId, int pharmacyId)
        {
            var today = DateTime.Today;
                
            return await _context.MedicationSubstitutes
                .AsNoTracking()
                .Where(ms => ms.PrimaryMedicationId == medicationId)
                .Join(
                    _context.PharmacyMedications,
                    ms => ms.AlternativeMedicationId,
                    pm => pm.MedicationId,
                    (ms, pm) => new { ms, pm }
                )
                .Where(x =>
                    x.pm.PharmacyId == pharmacyId &&
                    x.pm.Quantity > 0 &&
                    (x.pm.ExpiryDate == null || x.pm.ExpiryDate >= today)
                )
                .Select(x => new MedicationSubstituteDto
                {
                    Id = x.pm.MedicationId,
                    Name = x.pm.Medication.Name,
                })
                .Distinct()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Medication medication)
        {
            await _medicationRepo.AddAsync(medication);
            await _context.SaveChangesAsync();
        }
    
    }
}