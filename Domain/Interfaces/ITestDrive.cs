using CRM_AutoFlow.Application.DTOs;

namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface ITestDrive
    {
        Task<List<DateTime>> GetAvailableDaysAsync(Guid carId);
        Task<List<TimeOnly>> GetAvailableSlotsAsync(Guid carId, DateOnly date);
        Task AddTestDrive(TestDriveDTO testDriveDto);
    }
}
