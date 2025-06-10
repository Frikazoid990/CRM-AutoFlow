using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Infrastructure.Persistence;
using CRM_AutoFlow.Presentation.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class TestDriveService : ITestDrive
    {
        private readonly AppDbContext _context;

        public TestDriveService(AppDbContext context)
        {
            _context = context;
        }

        public Task AddTestDrive(TestDriveDTO testDriveDto)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DateTime>> GetAvailableDaysAsync(Guid carId)
        {
            var availableDays = new List<DateTime>();
            var today = DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(TimeOnly.MinValue);
            var endDate = today.AddYears(1);

            for (var date = today.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (await HasAvailableSlotAsync(carId, DateOnly.FromDateTime(date)))
                {
                    availableDays.Add(date);
                }
            }

            return availableDays;
        }

        public async Task<List<TimeOnly>> GetAvailableSlotsAsync(Guid carId, DateOnly date)
        {
            var MaxManagers = 6;
            var startDate = CombineDateAndTime(date, TimeOnly.MinValue);
            var endDate = startDate.AddDays(1);

            var result = new List<TimeOnly>();

            var existingDrives = await _context.TestDrives
                .Where(td =>
                    td.CarId == carId &&
                    td.Status != TestDriveStatus.CANCELED &&
                    td.PlannedDate >= startDate &&
                    td.PlannedDate < endDate)
                .ToListAsync();

            var startOfWorkDay = new TimeOnly(9, 0);
            var endOfWorkDay = new TimeOnly(20, 0);
            var current = startOfWorkDay;

            while (current < endOfWorkDay)
            {
                var slotStart = CombineDateAndTime(date, current);
                var slotEnd = CombineDateAndTime(date, current.AddMinutes(30));

                var carConflict = existingDrives.Any(td =>
                    td.CarId == carId &&
                    td.PlannedDate >= slotStart &&
                    td.PlannedDate < slotEnd);

                var staffBookings = existingDrives.Count(td =>
                    td.PlannedDate >= slotStart &&
                    td.PlannedDate < slotEnd);

                if (!carConflict && staffBookings < MaxManagers)
                {
                    result.Add(current);
                }

                current = current.AddMinutes(30);
            }

            return result;
        }

        private async Task<bool> HasAvailableSlotAsync(Guid carId, DateOnly date)
        {
            var MaxManagers = 6;
            var startDate = CombineDateAndTime(date, TimeOnly.MinValue);
            var endDate = startDate.AddDays(1);

            var existingDrives = await _context.TestDrives
                .Where(td =>
                    td.CarId == carId &&
                    td.Status != TestDriveStatus.CANCELED &&
                    td.PlannedDate >= startDate &&
                    td.PlannedDate < endDate)
                .ToListAsync();

            var startOfWorkDay = new TimeOnly(9, 0);
            var endOfWorkDay = new TimeOnly(20, 0);

            var current = startOfWorkDay;

            while (current < endOfWorkDay)
            {
                var slotStart = CombineDateAndTime(date, current);
                var slotEnd = CombineDateAndTime(date, current.AddMinutes(30));

                var carConflict = existingDrives.Any(td =>
                    td.CarId == carId &&
                    td.PlannedDate >= slotStart &&
                    td.PlannedDate < slotEnd);

                var staffBookings = existingDrives.Count(td =>
                    td.PlannedDate >= slotStart &&
                    td.PlannedDate < slotEnd);

                if (!carConflict && staffBookings < MaxManagers)
                {
                    return true;
                }

                current = current.AddMinutes(30);
            }

            return false;
        }

        private static DateTime CombineDateAndTime(DateOnly date, TimeOnly time)
        {
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                time.Hour,
                time.Minute,
                time.Second,
                DateTimeKind.Utc); // <-- Важно: указываем DateTimeKind.Utc
        }
    }
}
