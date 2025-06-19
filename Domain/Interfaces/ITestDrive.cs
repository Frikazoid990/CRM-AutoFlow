using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Application.Helper;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface ITestDrive
    {
        Task<List<ResponseTestDriveDTO>> GetAllTestDrive();
        Task<ResponseTestDriveDTO> GetTestDrive(Guid testDriveId);
        Task<List<ResponseTestDriveDTO>> GetTestDriveForClient(Guid clientId);
        Task<List<ResponseTestDriveDTO>> GetTestDriveForManager(Guid managerId);
        Task<List<DateTime>> GetAvailableDaysAsync(Guid carId);
        Task<List<TimeOnly>> GetAvailableSlotsAsync(Guid carId, DateOnly date);
        Task<Result<Guid>> AddTestDrive(CreateTestDriveDTO testDriveDto);
        Task UpdateWithEmployeeTestDrive(Guid testDriveId, Guid employeeId);
        Task UpdateWithStatusTestDrive(Guid testDriveId, TestDriveStatus status);



    }
}
