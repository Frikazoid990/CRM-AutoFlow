using CRM_AutoFlow.Application.DTOs;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface IManagerService
    {
        public Task<List<StaffDto>> GetStaffDealership();

        public Task<StatsManagerDto> GetStatsForManager(Guid managerId);
    }
}
