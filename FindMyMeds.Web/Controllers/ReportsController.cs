using FindMyMeds.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FindMyMeds.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportManager _reportManager;

        public ReportsController(IReportManager reportManager)
        {
            _reportManager = reportManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Stats = await _reportManager.GetDashboardStatsAsync();
            ViewBag.MedicationReport = await _reportManager.GetMedicationAvailabilityReportAsync();
            ViewBag.PharmacyReport = await _reportManager.GetPharmacyActivityReportAsync();
            return View();
        }

    }
}
