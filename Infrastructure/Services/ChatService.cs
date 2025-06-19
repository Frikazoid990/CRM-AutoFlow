using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;
        public ChatService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateChatAsync(Guid clientId)
        {
            var client = await _context.Users
                .Include(c => c.ClientChats)
                .FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null || client.Role != Role.CLIENT)
            {
                throw new ArgumentException("Client not found");
            }
            if (client.ClientChats.Count > 0)
            {
                throw new ArgumentException("The client already has a chat with the manager");
            }
            var chat = new Chat
            {
                ClientId = clientId,
                Title = "Chat",
            };
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return chat.Id;
        }
    }
}
