using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM_AutoFlow.Presentation.Controllers
{
    [Route("test-drive")]
    [ApiController]
    [Authorize]
    public class TestDriveController : ControllerBase
    {
        private readonly ITestDrive _testDriveService;

        public TestDriveController(ITestDrive testDriveService)
        {
            _testDriveService = testDriveService;
        }

        [HttpGet("available-days")]
        public async Task<IActionResult> GetAvailableDays([FromQuery] Guid carId)
        {
            var days = await _testDriveService.GetAvailableDaysAsync(carId);
            return Ok(days);
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] Guid carId, [FromQuery] DateOnly date)
        {
            var slots = await _testDriveService.GetAvailableSlotsAsync(carId, date);
            return Ok(slots);
        }

    }
}
