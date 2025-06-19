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
        public async Task AddMessage(CreateMessageDto messageDto)
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
        }

        public async Task<List<ResponseMessageDTO>> GetAllMessagesForChat(int chatId)
        {
            var chat = await _context.Chats.FindAsync(chatId);
            if (chat == null)
                throw new ArgumentException("Chat not found");
            var messageList = await _context.Messages
                .Where(m => m.ChatId == chatId)
                .Include(m => m.Sender)
                .ToListAsync();
            if (messageList.Count == 0)
                return new List<ResponseMessageDTO>();
            var messageDtoList = await Task.Run(() => messageList.Select(m => new ResponseMessageDTO
            {
                Id = m.Id,
                Content = m.Content,
                Sendler = m.Sender.FullName,
            })
                .ToList()
                );
            return messageDtoList;
        }
    }
}
