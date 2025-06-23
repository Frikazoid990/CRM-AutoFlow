using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_AutoFlow.Presentation.Controllers
{
    [Route("deal")]
    [ApiController]
    //[Authorize]
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
        public async Task<IActionResult> UpdateWithEmployeeDeal([FromRoute] Guid dealId, [FromBody] Guid employeeId)
        {
            await _dealService.UpdateWithEmploeeDeal(dealId, employeeId);
            return Ok("Employee update");
        }
        [HttpPatch("update-deal-with-status/{dealId}")]
        public async Task<IActionResult> UpdateWithStatusDeal([FromRoute] Guid dealId, [FromBody] DealStatus status)
        {
            await _dealService.UpdateWithStatusDeal(dealId, status);
            return Ok("Status update");
        }
        [HttpPatch("update-deal-with-is-canceled/{dealId}")]
        public async Task<IActionResult> UpdateWithIsCanceledDeal([FromRoute] Guid dealId)
        {
            await _dealService.UpdateWithIsCanceled(dealId);
            return Ok();
        }
        [HttpGet("get-all-deals")]
        public async Task<IActionResult> GetAllDeals()
        {
            var result = await _dealService.GetAllDeals();
            return Ok(result);
        }
        [HttpGet("get-deal-for-client/{clientId}")]
        public async Task<IActionResult> GetDealForClient([FromRoute] Guid clientId)
        {
            var result = await _dealService.GetDealForCliet(clientId);
            return Ok(result);
        } 

    }
}
