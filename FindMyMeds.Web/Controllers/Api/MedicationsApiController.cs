using FindMyMeds.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FindMyMeds.Web.Controllers.Api
{
    [ApiController]
    [Route("api/medications")]
    public class MedicationsApiController : ControllerBase
    {
        private readonly IMedicationManager _medicationManager;

        public MedicationsApiController(IMedicationManager medicationManager)
        {
            _medicationManager = medicationManager;
        }

        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return BadRequest("Query must be at least 2 characters.");

            var results = await _medicationManager.SearchAsync(q);
            return Ok(results);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(int id)
        {
            var med = await _medicationManager.GetDetailsAsync(id);
            if (med == null)
                return NotFound();

            return Ok(med);
        }
    }
}
