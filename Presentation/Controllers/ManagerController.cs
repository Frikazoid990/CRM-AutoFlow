using CRM_AutoFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_AutoFlow.Presentation.Controllers
{
    [ApiController]
    [Route("manager")]
    [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR")]
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;

        public ManagerController (IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet("get-staff")]
        public async Task<IActionResult> GetStaffDealerShip()
        {
            var result = await _managerService.GetStaffDealership();
            return Ok(result);
        }
    }
}
