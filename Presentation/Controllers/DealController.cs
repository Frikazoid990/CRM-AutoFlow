using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CRM_AutoFlow.Presentation.Controllers
{
    [Route("deal")]
    [ApiController]
    [Authorize]
    public class DealController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IDealService _dealService;
        public DealController(IChatService chatService, IDealService dealService) 
        {
            _chatService = chatService;
            _dealService = dealService;
        }

        [HttpPost("chat/test")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateChat([FromBody] Guid clientId)
        {
            var result = await _chatService.CreateChatAsync(clientId);
            return Ok(result);
        }

        [HttpPost("add-deal")]
        public async Task<IActionResult> AddDeal([FromBody] CreateDealDTO dealDto)
        {
            var result = await _dealService.AddDeal(dealDto);
            return Ok(result);
        }
        [HttpPatch("update-deal-with-employee/{dealId}")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR")]
        public async Task<IActionResult> UpdateWithEmployeeDeal([FromRoute] Guid dealId, [FromBody] Guid employeeId)
        {
            await _dealService.UpdateWithEmploeeDeal(dealId, employeeId);
            return Ok("Employee update");
        }
        [HttpPatch("update-deal-with-status/{dealId}")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,MANAGER,DIRECTOR")]
        public async Task<IActionResult> UpdateWithStatusDeal([FromRoute] Guid dealId, [FromBody] DealStatus status)
        {
            await _dealService.UpdateWithStatusDeal(dealId, status);
            return Ok("Status update");
        }
        [HttpPatch("update-deal-with-is-canceled/{dealId}")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,MANAGER,DIRECTOR")]
        public async Task<IActionResult> UpdateWithIsCanceledDeal([FromRoute] Guid dealId)
        {
            await _dealService.UpdateWithIsCanceled(dealId);
            return Ok();
        }
        [HttpPatch("update-current-deal-price/{dealId}")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,MANAGER,DIRECTOR")]
        public async Task<IActionResult> UpdateCurrentPriceDeal([FromRoute] Guid dealId, [FromBody] decimal price)
        {
            await _dealService.UpdateCurrentDealPrice(dealId, price);
            return Ok();
        }

        [HttpGet("get-all-deals")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR,MANAGER")]
        public async Task<IActionResult> GetAllDeals()
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

            var result = await _dealService.GetAllDeals(userId, role);
            return Ok(result);
        }
        [HttpGet("get-deal-for-client/{clientId}")]
        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> GetDealForClient([FromRoute] Guid clientId)
        {
            var result = await _dealService.GetDealForCliet(clientId);
            return Ok(result);
        }
        [HttpGet("get-deals-for-manager/{managerId}")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,MANAGER,DIRECTOR")]
        public async Task<IActionResult> GetAllDealsForCurrentManager([FromRoute] Guid managerId)
        {
            var result = await _dealService.GetAllDealsForCurrentManager(managerId);
            return Ok(result);
        }
        [HttpGet("get-deal-is-canceled")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR")]
        public async Task<IActionResult> GetAllDealsWithIsCanceled()
        {
            var result = await _dealService.GetAllIsCanceledDeal();
            return Ok(result);
        }

        [HttpGet("get-current-deal/{dealId}")]
        [Authorize(Roles = "ADMIN,SENIORMANAGER,DIRECTOR,MANAGER")]
        public async Task<IActionResult> GetCurrentDeal([FromRoute] Guid dealId)
        {
            var result = await _dealService.GetCurrentDeal(dealId);
            return Ok(result);
        }


        

    }
}
