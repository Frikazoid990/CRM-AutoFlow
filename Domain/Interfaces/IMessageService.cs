using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Models;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface IMessageService
    {
        public Task<List<ResponseMessageDTO>> GetAllMessagesForChat(int chatId);
        public Task<Guid> AddMessage (CreateMessageDto message);
    }
}
