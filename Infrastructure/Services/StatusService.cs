using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Presentation.Extensions;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class StatusService : IStatusService
    {
        public List<TestDriveStatusDTO> GetTestDrivesStatus()
        {
            var statuses = Enum.GetValues<TestDriveStatus>()
                .Select(s => new TestDriveStatusDTO
                {
                    Value = s,
                    Label = s.GetDescription(),
                })
                .ToList();
            return(statuses);
        }
        public List<DealStatusDTO> GetDealsStatus()
        {
            var statuses = Enum.GetValues<DealStatus>()
                .Select(s => new DealStatusDTO
                {
                    Value = s,
                    Label = s.GetDescription(),
                })
                .ToList();
            return(statuses);
        }
    }
}
