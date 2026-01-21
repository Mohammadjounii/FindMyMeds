using FindMyMeds.Core.Entities;
using FindMyMeds.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Linq;
using System.Security.Claims;
using FindMyMeds.Core.ViewModels;

namespace FindMyMeds.Web.Controllers
{
    [Authorize(Roles = "Pharmacy")]
    public class PharmacyMedicationsController : Controller
    {
        private readonly IPharmacyMedicationManager _inventoryManager;
        private readonly IPharmacyManager _pharmacyManager;
        private readonly IMedicationManager _medicationManager;
        private readonly ILogger<PharmacyMedicationsController> _logger;

        public PharmacyMedicationsController(
            IPharmacyMedicationManager inventoryManager,
            IPharmacyManager pharmacyManager,
            IMedicationManager medicationManager,
            ILogger<PharmacyMedicationsController> logger)
        {
            _inventoryManager = inventoryManager;
            _pharmacyManager = pharmacyManager;
            _medicationManager = medicationManager;
            _logger = logger;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        private async Task<Pharmacy?> GetOwnedApprovedPharmacyOrNull(int pharmacyId)
        {
            var pharmacy = await _pharmacyManager.GetByIdAsync(pharmacyId);
            if (pharmacy == null) return null;
            if (!pharmacy.IsApproved) return null;
            if (pharmacy.OwnerUserId != CurrentUserId) return null;
            return pharmacy;
        }
        public async Task<IActionResult> Index(int pharmacyId)
        {
            var pharmacy = await _pharmacyManager.GetByIdAsync(pharmacyId);
            if (pharmacy == null)
                return NotFound();

            ViewBag.Pharmacy = pharmacy;

            var items = await _inventoryManager.GetByPharmacyIdAsync(pharmacyId);

            var model = items.Select(pm => new PharmacyMedicationViewModel
            {
                Id = pm.Id,
                PharmacyId = pm.PharmacyId,
                PharmacyName = pharmacy.Name,
                MedicationId = pm.MedicationId,
                MedicationName = pm.Medication!.Name,
                Quantity = pm.Quantity,
                Price = pm.Price,
                ExpiryDate = pm.ExpiryDate   
            }).ToList();

            return View(model);
        }
        public async Task<IActionResult> Create(int pharmacyId)
        {
            var pharmacy = await GetOwnedApprovedPharmacyOrNull(pharmacyId);
            if (pharmacy == null) return RedirectToAction("PendingApproval", "Pharmacies");

            ViewBag.Pharmacy = pharmacy;
            ViewBag.Medications = await _medicationManager.GetAllForDropdownAsync();

            return View(new PharmacyMedication { PharmacyId = pharmacyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PharmacyMedication pm)
        {
            var pharmacy = await GetOwnedApprovedPharmacyOrNull(pm.PharmacyId);
            if (pharmacy == null)
                return RedirectToAction("PendingApproval", "Pharmacies");

            if (pm.PharmacyId <= 0)
                ModelState.AddModelError(nameof(pm.PharmacyId), "Invalid pharmacy.");

            if (pm.MedicationId <= 0)
                ModelState.AddModelError(nameof(pm.MedicationId), "Please select a medication");

            if (pm.Quantity <= 0)
                ModelState.AddModelError(nameof(pm.Quantity), "Quantity must be greater than 0.");

            if (pm.Price <= 0)
                ModelState.AddModelError(nameof(pm.Price), "Price must be greater than 0.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogWarning(
                    "Create PharmacyMedication failed validation | PharmacyId={PharmacyId} | Errors={Errors}",
                    pm.PharmacyId,
                    string.Join(" | ", errors));

                ViewBag.ErrorList = errors;
                ViewBag.DebugPosted = pm;

                ViewBag.Pharmacy = pharmacy;
                ViewBag.Medications = await _medicationManager.GetAllForDropdownAsync();
                return View(pm);
            }

            var existing = await _inventoryManager.GetByPharmacyAndMedicationAsync(pm.PharmacyId, pm.MedicationId);

            if (existing != null)
            {
                _logger.LogWarning(
                    "Duplicate medication attempt | PharmacyId={PharmacyId}, MedicationId={MedicationId}",
                    pm.PharmacyId,
                    pm.MedicationId);

                if (existing.Price == pm.Price)
                {
                    ModelState.AddModelError(nameof(pm.Quantity),
                        "This medication already exists. Increase the quantity instead.");
                }
                else
                {
                    ModelState.AddModelError(nameof(pm.Price),
                        $"This medication already exists with a different price ({existing.Price:0.00}). Edit the existing entry instead.");
                }

                ViewBag.Pharmacy = pharmacy;
                ViewBag.Medications = await _medicationManager.GetAllForDropdownAsync();
                return View(pm);
            }

            await _inventoryManager.AddAsync(pm);

            using (LogContext.PushProperty("Category", "Inventory"))
            {
                _logger.LogInformation(
                    "Medication added to pharmacy | PharmacyId={PharmacyId}, MedicationId={MedicationId}, Quantity={Quantity}, Price={Price}",
                    pm.PharmacyId,
                    pm.MedicationId,
                    pm.Quantity,
                    pm.Price);
            }

            return RedirectToAction("Dashboard", "Pharmacies");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _inventoryManager.GetAsync(id);
            if (item == null) return NotFound();

            var pharmacy = await GetOwnedApprovedPharmacyOrNull(item.PharmacyId);
            if (pharmacy == null) return RedirectToAction("PendingApproval", "Pharmacies");

            ViewBag.Pharmacy = pharmacy;
            ViewBag.Medications = await _medicationManager.GetAllForDropdownAsync();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PharmacyMedication pm)
        {
            var pharmacy = await GetOwnedApprovedPharmacyOrNull(pm.PharmacyId);
            if (pharmacy == null) return RedirectToAction("PendingApproval", "Pharmacies");

            if (pm.MedicationId <= 0)
                ModelState.AddModelError(nameof(pm.MedicationId), "Please select a medication");

            if (pm.Quantity <= 0)
                ModelState.AddModelError(nameof(pm.Quantity), "Quantity must be greater than 0.");

            if (pm.Price <= 0)
                ModelState.AddModelError(nameof(pm.Price), "Price must be greater than 0.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Edit PharmacyMedication invalid ModelState: {Errors}", string.Join(" | ", errors));
                ViewBag.ErrorList = errors;
                ViewBag.DebugPosted = pm;

                ViewBag.Pharmacy = pharmacy;
                ViewBag.Medications = await _medicationManager.GetAllForDropdownAsync();
                return View(pm);
            }

            var existing = await _inventoryManager.GetByPharmacyAndMedicationAsync(pm.PharmacyId, pm.MedicationId);

            if (existing != null && existing.Id != pm.Id)
            {
                if (existing.Price == pm.Price)
                {
                    ModelState.AddModelError(nameof(pm.Quantity),
                        "Another entry of this medication exists. Update its quantity instead.");
                }
                else
                {
                    ModelState.AddModelError(nameof(pm.Price),
                        $"Another entry exists with a different price ({existing.Price:0.00}). Edit that entry instead.");
                }

                ViewBag.DebugPosted = pm;
                ViewBag.DebugExisting = existing;

                ViewBag.Pharmacy = pharmacy;
                ViewBag.Medications = await _medicationManager.GetAllForDropdownAsync();
                return View(pm);
            }

            await _inventoryManager.UpdateAsync(pm);

            return RedirectToAction("Dashboard", "Pharmacies");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _inventoryManager.GetAsync(id);
            if (item == null) return NotFound();

            var pharmacy = await GetOwnedApprovedPharmacyOrNull(item.PharmacyId);
            if (pharmacy == null) return RedirectToAction("PendingApproval", "Pharmacies");

            ViewBag.Pharmacy = pharmacy;
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _inventoryManager.GetAsync(id);
            if (item == null) return NotFound();

            var pharmacy = await GetOwnedApprovedPharmacyOrNull(item.PharmacyId);
            if (pharmacy == null) return RedirectToAction("PendingApproval", "Pharmacies");

            await _inventoryManager.DeleteAsync(id);

            return RedirectToAction("Dashboard", "Pharmacies");
        }
    
        [HttpGet]
        public async Task<IActionResult> SearchMedications(string term)
        {
            var meds = string.IsNullOrWhiteSpace(term)
                ? await _medicationManager.GetAllForDropdownAsync()
                : await _medicationManager.SearchForDropdownAsync(term);

            return Json(meds.Select(m => new
            {
                id = m.Id,
                text = m.Name
            }));
        }



    }
}
