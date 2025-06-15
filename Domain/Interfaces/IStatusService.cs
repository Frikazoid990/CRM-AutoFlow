using CRM_AutoFlow.Application.DTOs;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface IStatusService
    {
        public List<TestDriveStatusDTO> GetTestDrivesStatus();
        public List<DealStatusDTO> GetDealsStatus();
    }
}
