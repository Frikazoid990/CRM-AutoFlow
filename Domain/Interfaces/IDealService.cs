using CRM_AutoFlow.Application.DTOs;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface IDealService
    {
        public Task<ResponseDealDTO> GetCurrentDeal(Guid dealId);
        public Task<Guid> AddDeal(CreateDealDTO dealDto);

        public Task UpdateWithEmploeeDeal(Guid dealId, Guid emploeeId);

        public Task UpdateWithStatusDeal(Guid dealId, DealStatus status);

        public Task UpdateCurrentDealPrice(Guid dealId, decimal price);

        public Task<List<ResponseDealDTO>> GetAllDeals(Guid userId, string role);

        public Task UpdateWithIsCanceled(Guid dealId);

        public Task<ResponseDealDTO> GetDealForCliet(Guid clientId);

        public Task<List<ResponseDealDTO>> GetAllIsCanceledDeal();

        public Task<List<ResponseDealDTO>> GetAllDealsForCurrentManager(Guid managerId);
    }
}
