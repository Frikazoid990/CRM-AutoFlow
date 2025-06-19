using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Models;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface IChatService
    {
        public Task<int> CreateChatAsync(Guid clientId);
    }
}
