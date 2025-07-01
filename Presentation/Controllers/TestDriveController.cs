using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        /// for employees or high staff
        [HttpGet("get-test-drives")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR,MANAGER")]
        public async Task<IActionResult> GetAllTestDrives() 
        {
            string token = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return BadRequest("Invalid token");

            // Теперь можем получить claim'ы
            var userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
            var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var phoneNumber = jwtToken.Claims.FirstOrDefault(c => c.Type == "phoneNumber")?.Value;
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var result = await _testDriveService.GetAllTestDrive(userId, role);
            if (result.Count == 0)
            {
                return NotFound("Test-drives not found");
            }
            return Ok(result);
        }
        [HttpGet("get-test-drives-for-manager/{managerId}")]
        public async Task<IActionResult> GetTestDrivesForManager(Guid managerId)
        {
            var result = await _testDriveService.GetTestDriveForManager(managerId);
            return Ok(result);
        }
        [HttpPatch("test-drive-update-employee/{testDriveId}")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR")]
        public async Task<IActionResult> UpdateWithEmployeeTestDrive([FromRoute] Guid testDriveId, [FromBody]  Guid employeeId)
        {
            await _testDriveService.UpdateWithEmployeeTestDrive(testDriveId, employeeId);
            return Ok($"Test drive {testDriveId} is update");
        }
        [HttpPatch("test-drive-update-status/{testDriveId}")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,MANAGER,DIRECTOR")]
        public async Task<IActionResult> UpdateWithStatusTestDrive([FromRoute] Guid testDriveId, [FromBody] TestDriveStatus status)
        {
            await _testDriveService.UpdateWithStatusTestDrive(testDriveId, status);
            return Ok($"Test drive {testDriveId} is update");
        }
        ///for all users, who authorize
        [HttpGet("get-test-drives-for-client/{clientId}")]
        public async Task<IActionResult> GetTestDrivesForClient([FromRoute] Guid clientId)
        {
            var result = await _testDriveService.GetTestDriveForClient(clientId);
            return Ok(result);
        }
        [HttpGet("get-test-drive/{testDriveId}")]
        public async Task<IActionResult> GetTestDrive([FromRoute] Guid testDriveId)
        {
            var result = await _testDriveService.GetTestDrive(testDriveId);
            return Ok(result);
        }
        [HttpPost("add-test-drive")]
        public async Task<IActionResult> AddTestDrive([FromBody] CreateTestDriveDTO testDriveDto)
        {
            var result = await _testDriveService.AddTestDrive(testDriveDto);
            return Ok(result.Data);
        }

        [HttpGet("available-days")]
        [Authorize(Roles = "ADMIN")]
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
