using FindMyMeds.Core.DTOs;
using FindMyMeds.Core.Entities;
using FindMyMeds.Core.ViewModels;
using FindMyMeds.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindMyMeds.Web.Controllers
{
    public class MedicationsController : Controller
    {
        private readonly IMedicationManager _medicationManager;
        private readonly IPharmacyManager _pharmacyManager;

        public MedicationsController(
            IMedicationManager medicationManager,
            IPharmacyManager pharmacyManager)
        {
            _medicationManager = medicationManager;
            _pharmacyManager = pharmacyManager;
        }
        public async Task<IActionResult> Search(string q, string? city, string? sort = "name")
        {
            var pharmacies = await _pharmacyManager.GetAllAsync();
            ViewBag.Cities = pharmacies
                .Select(p => p.City)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            if (string.IsNullOrWhiteSpace(q))
                return View(new List<MedicationAvailabilityViewModel>());

            var results = await _medicationManager.SearchAsync(q);

            if (!string.IsNullOrEmpty(city) && city != "All")
                results = results.Where(r => r.City == city).ToList();

            var vm = results.Select(r => new MedicationAvailabilityViewModel
            {
                MedicationId = r.MedicationId,
                MedicationName = r.MedicationName,
                PharmacyId = r.PharmacyId,
                PharmacyName = r.PharmacyName,
                City = r.City,
                Quantity = r.Quantity,
                Price = r.Price
            }).ToList();

            sort = sort?.ToLower();
            vm = sort switch
            {
                "lowprice" => vm.OrderBy(r => r.Price).ToList(),
                "highprice" => vm.OrderByDescending(r => r.Price).ToList(),
                "highqty" => vm.OrderByDescending(r => r.Quantity).ToList(),
                _ => vm.OrderBy(r => r.MedicationName).ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id, int? pharmacyId)
        {
            var details = await _medicationManager.GetDetailsAsync(id);
            if (details == null)
                return NotFound();

            if (pharmacyId.HasValue && pharmacyId.Value > 0)
            {
                ViewBag.Substitutes =
                    await _medicationManager
                        .GetAvailableSubstitutesInPharmacyAsync(id, pharmacyId.Value);
            }
            else
            {
                ViewBag.Substitutes = new List<MedicationSubstituteDto>();
            }

            return PartialView("_MedicationDetailsPartial", details);
        }

        [HttpGet]
        public async Task<IActionResult> LiveSearch(string q, string? city, string? sort = "name")
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return PartialView("_LiveSearchResults",
                    new List<MedicationAvailabilityViewModel>());
            }

            var results = await _medicationManager.SearchAsync(q);

            if (!string.IsNullOrEmpty(city) && city != "All")
                results = results.Where(r => r.City == city).ToList();

            var vm = results.Select(r => new MedicationAvailabilityViewModel
            {
                MedicationId = r.MedicationId,
                MedicationName = r.MedicationName,
                PharmacyId = r.PharmacyId,
                PharmacyName = r.PharmacyName,
                City = r.City,
                Quantity = r.Quantity,
                Price = r.Price
            }).ToList();

            sort = sort?.ToLower();
            vm = sort switch
            {
                "lowprice" => vm.OrderBy(r => r.Price).ToList(),
                "highprice" => vm.OrderByDescending(r => r.Price).ToList(),
                "highqty" => vm.OrderByDescending(r => r.Quantity).ToList(),
                _ => vm.OrderBy(r => r.MedicationName).ToList()
            };

            return PartialView("_LiveSearchResults", vm);
        }
    }
}
