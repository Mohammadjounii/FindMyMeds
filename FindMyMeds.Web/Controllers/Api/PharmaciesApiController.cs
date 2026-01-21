using FindMyMeds.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FindMyMeds.Web.Controllers.Api
{
    [ApiController]
    [Route("api/pharmacies")]
    public class PharmaciesApiController : ControllerBase
    {
        private readonly IPharmacyManager _pharmacyManager;
        private readonly IPharmacyMedicationManager _inventoryManager;

        public PharmaciesApiController(
            IPharmacyManager pharmacyManager,
            IPharmacyMedicationManager inventoryManager)
        {
            _pharmacyManager = pharmacyManager;
            _inventoryManager = inventoryManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovedPharmacies()
        {
            var pharmacies = (await _pharmacyManager.GetAllAsync())
                .Where(p => p.IsApproved)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.City,
                    p.Phone
                });

            return Ok(pharmacies);
        }

        [HttpGet("{pharmacyId:int}/inventory")]
        public async Task<IActionResult> GetInventory(int pharmacyId)
        {
            var inventory = await _inventoryManager.GetByPharmacyIdAsync(pharmacyId);

            var result = inventory
                .Where(i =>
                 i.Quantity > 0 &&
                (!i.ExpiryDate.HasValue || i.ExpiryDate.Value.Date >= DateTime.UtcNow.Date)
                )

                .Select(i => new
                {
                    MedicationId = i.MedicationId,
                    MedicationName = i.Medication!.Name,
                    i.Quantity,
                    i.Price,
                    i.ExpiryDate
                });

            return Ok(result);
        }
    }
}
