using FindMyMeds.Core.Entities;
using FindMyMeds.Core.Identity;
using FindMyMeds.Infrastructure.Data;
using FindMyMeds.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;
using System.Security.Claims;

namespace FindMyMeds.Web.Controllers
{
    public class PharmaciesController : Controller
    {
        private readonly IPharmacyManager _manager;
        private readonly IPharmacyMedicationManager _inventoryManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PharmaciesController> _logger;
        private readonly ApplicationDbContext _context;

        public PharmaciesController(
            IPharmacyManager manager,
            IPharmacyMedicationManager inventoryManager,
            UserManager<ApplicationUser> userManager,
            ILogger<PharmaciesController> logger,
            ApplicationDbContext context)
        {
            _manager = manager;
            _inventoryManager = inventoryManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var dbName = _context.Database.GetDbConnection().Database;
            _logger.LogError("ADMIN DEBUG → DATABASE = {Db}", dbName);

            var count = await _context.Pharmacies.CountAsync();
            _logger.LogError("ADMIN DEBUG → PHARMACY COUNT = {Count}", count);

            var list = await _context.Pharmacies
                .Include(p => p.OwnerUser)
                .ToListAsync();

            return View(list);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Admin opened edit pharmacy page | PharmacyId={PharmacyId}", id);

            var ph = await _manager.GetByIdAsync(id);
            if (ph == null) return NotFound();

            return View(ph);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Pharmacy model, IFormFile? Logo)
        {
            if (!ModelState.IsValid)
                return View(model);

            var pharmacy = await _manager.GetByIdAsync(model.Id);
            if (pharmacy == null)
                return NotFound();

            pharmacy.Name = model.Name;
            pharmacy.City = model.City;
            pharmacy.Phone = model.Phone;

            if (Logo != null && Logo.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/pharmacies"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(pharmacy.LogoPath))
                {
                    var oldPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        pharmacy.LogoPath.TrimStart('/')
                    );

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(Logo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await Logo.CopyToAsync(stream);

                pharmacy.LogoPath = "/uploads/pharmacies/" + fileName;
            }

            await _manager.UpdateAsync(pharmacy);

            using (LogContext.PushProperty("Category", "Pharmacy"))
            {
                _logger.LogInformation(
                    "Pharmacy edited by admin | PharmacyId={PharmacyId}",
                    pharmacy.Id);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ph = await _manager.GetByIdAsync(id);
            if (ph == null) return NotFound();

            return View(ph);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _manager.DeleteAsync(id);

            using (LogContext.PushProperty("Category", "Pharmacy"))
            {
                _logger.LogWarning(
                    "Pharmacy deleted by admin | PharmacyId={PharmacyId}",
                    id);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            await _manager.ApproveAsync(id);

            var pharmacy = await _manager.GetByIdAsync(id);
            if (pharmacy?.OwnerUserId != null)
            {
                var user = await _userManager.FindByIdAsync(pharmacy.OwnerUserId);
                if (user != null && !await _userManager.IsInRoleAsync(user, "Pharmacy"))
                {
                    await _userManager.AddToRoleAsync(user, "Pharmacy");
                }
            }

            using (LogContext.PushProperty("Category", "Pharmacy"))
            {
                _logger.LogInformation(
                    "Pharmacy approved by admin | PharmacyId={PharmacyId}",
                    id);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Index));

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Pharmacy pharmacy, IFormFile? Logo)
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
                return View(pharmacy);

            pharmacy.OwnerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            pharmacy.IsApproved = false;

            if (Logo != null && Logo.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/pharmacies"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(Logo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await Logo.CopyToAsync(stream);

                pharmacy.LogoPath = "/uploads/pharmacies/" + fileName;
            }

            await _manager.AddAsync(pharmacy);

            return RedirectToAction(nameof(PendingApproval));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id, string? returnUrl)
        {
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl)
                ? Url.Action("Search", "Medications")
                : returnUrl;

            var pharmacy = await _manager.GetByIdAsync(id);
            if (pharmacy == null)
                return NotFound();

            ViewBag.Inventory = await _inventoryManager.GetByPharmacyIdAsync(id);
            return View(pharmacy);
        }

        [Authorize]
        public IActionResult PendingApproval()
        {
            return View();
        }

        [Authorize(Roles = "Pharmacy")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var pharmacies = (await _manager.GetAllAsync())
                .Where(p => p.OwnerUserId == userId)
                .ToList();

            if (!pharmacies.Any())
                return RedirectToAction(nameof(PendingApproval));

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            ViewBag.Notifications = notifications;

            return View(pharmacies);
        }
    }
}
