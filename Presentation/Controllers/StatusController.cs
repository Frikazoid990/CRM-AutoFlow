using CRM_AutoFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_AutoFlow.Presentation.Controllers
{
    [ApiController]
    [Route("statuses")]
    [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR")]
    public class StatusController : Controller
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet("get-statuses-test-drives")]
        public IActionResult GetTestDriveStatus()
        {
            var result = _statusService.GetTestDrivesStatus();
            return Ok(result);
        }
        [HttpGet("get-statuses-deals")]
        public IActionResult GetDealsStatus()
        {
            var result = _statusService.GetDealsStatus();
            return Ok(result);
        }
    }
}
