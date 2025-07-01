using CRM_AutoFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_AutoFlow.Presentation.Controllers
{
    [ApiController]
    [Route("manager")]
    [Authorize]
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;

        public ManagerController (IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet("get-staff")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR,MANAGER")]
        public async Task<IActionResult> GetStaffDealerShip()
        {
            var result = await _managerService.GetStaffDealership();
            return Ok(result);
        }

        [HttpGet("get-stats/{managerId}")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,MANAGER,DIRECTOR")]
        public async Task<IActionResult> GetStatsFromCurrentManager([FromRoute] Guid managerId)
        {
            var result = await _managerService.GetStatsForManager(managerId);
            return Ok(result);
        }
    }
}
