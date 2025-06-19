using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM_AutoFlow.Presentation.Controllers
{
    [Route("message")]
    [ApiController]
    //[Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("get-message-for-chat/{chatId}")]
        public async Task<IActionResult> GetAllMessageForChat([FromRoute] int chatId)
        {
            var result = await _messageService.GetAllMessagesForChat(chatId);
            return Ok(result);
        }
        [HttpPost("add-message")]
        public async Task<IActionResult> AddMessage([FromBody] CreateMessageDto dto)
        {
            await _messageService.AddMessage(dto);
            return Ok();
        }
    }
}
