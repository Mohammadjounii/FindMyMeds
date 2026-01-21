using FindMyMeds.Core.Identity;
using FindMyMeds.Infrastructure.Data;
using FindMyMeds.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


[Authorize]
public class SettingsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public SettingsController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Admin"))
            ViewBag.Role = "Admin";
        else if (roles.Contains("Pharmacy"))
            ViewBag.Role = "Pharmacy";
        else
            ViewBag.Role = "User";

        using var scope = HttpContext.RequestServices.CreateScope();
        var pharmacyManager = scope.ServiceProvider.GetRequiredService<IPharmacyManager>();
            
        var pharmacies = (await pharmacyManager.GetAllAsync())
            .Where(p => p.OwnerUserId == user.Id)
            .ToList();

        ViewBag.Pharmacies = pharmacies;

        return View(user);
    }

}
