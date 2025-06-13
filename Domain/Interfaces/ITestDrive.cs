using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Application.Helper;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface ITestDrive
    {
        Task<List<TestDriveDTO>> GetAllTestDrive();
        Task<List<DateTime>> GetAvailableDaysAsync(Guid carId);
        Task<List<TimeOnly>> GetAvailableSlotsAsync(Guid carId, DateOnly date);
        Task<Result<Guid>> AddTestDrive(TestDriveDTO testDriveDto);
        Task UpdateEmployeeTestDrive(Guid testDriveId, Guid employeeId);
    }
}
