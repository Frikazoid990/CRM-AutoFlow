using CRM_AutoFlow.Application.DTOs;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface IDealService
    {
        public Task<Guid> AddDeal(CreateDealDTO dealDto);

        public Task UpdateWithEmploeeDeal(Guid dealId, Guid emploeeId);

        public Task UpdateWithStatusDeal(Guid dealId, DealStatus status);

        public Task<List<ResponseDealDTO>> GetAllDeals();

        public Task UpdateWithIsCanceled(Guid dealId);
    }
}
