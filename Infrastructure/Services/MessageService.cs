using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context)
        {
            _context = context;
        }
        ///----
        public async Task<Guid> AddMessage(CreateMessageDto messageDto)
        {
            if (messageDto.Content == null)
                throw new ArgumentException("Empty message");
            var message = new Message
            {
                Content = messageDto.Content,
                ChatId = messageDto.ChatId,
                SenderId = messageDto.SenderId,
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return(message.Id);
        }

        public async Task<List<ResponseMessageDTO>> GetAllMessagesForChat(int chatId)
        {
            return await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.CreatedAt)
                .Include(m => m.Sender)
                .Select(m => new ResponseMessageDTO
                {
                    Id = m.Id,
                    Content = m.Content,
                    Sendler = m.Sender.FullName,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();
        }
    }
}
