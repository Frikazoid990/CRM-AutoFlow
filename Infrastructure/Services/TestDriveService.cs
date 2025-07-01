using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Application.Helper;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Persistence;
using CRM_AutoFlow.Presentation.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
  
        public async Task<Result<Guid>> AddTestDrive(CreateTestDriveDTO testDriveDto)
        {
            var testDrive = testDriveDto.ToEntity();
            await _context.TestDrives.AddAsync(testDrive);
            await _context.SaveChangesAsync();
            return Result<Guid>.Ok(testDrive.Id);
        }

        public async Task UpdateWithEmployeeTestDrive(Guid testDriveId, Guid employeeId)
        {

            TestDrive testDrive = await _context.TestDrives.FindAsync(testDriveId);
            if (testDrive == null)
            {
                throw new InvalidOperationException($"Test drive with ID {testDriveId} not found");
            }
            testDrive.EmployeedId = employeeId;
            testDrive.Status = TestDriveStatus.INITIAL;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithStatusTestDrive(Guid testDriveId, TestDriveStatus status)
        {

            TestDrive testDrive = await _context.TestDrives.FindAsync(testDriveId);
            if (testDrive == null)
            {
                throw new InvalidOperationException($"Test drive with ID {testDriveId} not found");
            }
            testDrive.Status = status;
            testDrive.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
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

            var startOfWorkDay = new TimeOnly(10, 0);
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
                    if (current != new TimeOnly(13, 0) && current != new TimeOnly(13, 30))
                    {
                        result.Add(current);
                    }
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

        public async Task<List<ResponseTestDriveDTO>> GetAllTestDrive(Guid userId, string role)
        { 
            if(role == "MANAGER")
            {
                var nowUtc = DateTime.UtcNow;
                var nowMsk = nowUtc.AddHours(3); // Имитируем московское время

                // 2. Запрашиваем данные из БД — PlannedDate хранится как MSK
                var testDrives = await _context.TestDrives
                    .Where(t => (t.Status == TestDriveStatus.INITIAL) &&
                                t.PlannedDate >= nowMsk && // PlannedDate — это MSK
                                t.PlannedDate <= nowMsk.AddDays(365)
                                && t.EmployeedId == userId)
                    .Include(td => td.Car)
                    .Include(td => td.Client)
                    .Include(td => td.Employee)
                    .OrderBy(t => t.PlannedDate)
                    .ToListAsync();

                if (!testDrives.Any())
                    return new List<ResponseTestDriveDTO>();
                // 3. Конвертируем MSK → UTC для DTO (для отправки клиенту)
                var testDrivesListDto = testDrives.Select(t => new ResponseTestDriveDTO
                {
                    Id = t.Id,
                    Status = t.Status.GetDescription(),
                    PlannedDate = t.PlannedDate, // MSK → UTC
                    Car = t.Car.toShortInfo(),
                    Client = t.Client.toClientShortInfo(),
                    Employee = t.Employee?.toEmployeeShortInfo(),
                }).ToList();

                return testDrivesListDto;
            }
            else
            {
                var nowUtc = DateTime.UtcNow;
                var nowMsk = nowUtc.AddHours(3); // Имитируем московское время

                // 2. Запрашиваем данные из БД — PlannedDate хранится как MSK
                var testDrives = await _context.TestDrives
                    .Where(t => t.Status != TestDriveStatus.CANCELED &&
                                t.PlannedDate >= nowMsk && // PlannedDate — это MSK
                                t.PlannedDate <= nowMsk.AddDays(365))
                    .Include(td => td.Car)
                    .Include(td => td.Client)
                    .Include(td => td.Employee)
                    .OrderBy(t => t.PlannedDate)
                    .ToListAsync();

                if (!testDrives.Any())
                    return new List<ResponseTestDriveDTO>();
                // 3. Конвертируем MSK → UTC для DTO (для отправки клиенту)
                var testDrivesListDto = testDrives.Select(t => new ResponseTestDriveDTO
                {
                    Id = t.Id,
                    Status = t.Status.GetDescription(),
                    PlannedDate = t.PlannedDate, // MSK → UTC
                    Car = t.Car.toShortInfo(),
                    Client = t.Client.toClientShortInfo(),
                    Employee = t.Employee?.toEmployeeShortInfo(),
                }).ToList();

                return testDrivesListDto;
            }

                // 1. Получаем текущее время в UTC и добавляем +3 часа, чтобы получить "время по Москве"

        }

        public async Task<ResponseTestDriveDTO> GetTestDrive(Guid testDriveId)
        {
            var testDrive = await _context.TestDrives
                .Where(t => t.Id == testDriveId)
                .Include(t => t.Car)          
                .Include(t => t.Client)       
                .Include(t => t.Employee)     
                .FirstOrDefaultAsync();
            if(testDrive == null)
            {
                throw new InvalidOperationException($"Test drive with ID {testDriveId} not found");
            }
            return new ResponseTestDriveDTO
            {
                Id = testDrive.Id,
                Status = testDrive.Status.GetDescription(),
                PlannedDate = testDrive.PlannedDate,
                Car = testDrive.Car.toShortInfo(),
                Client = testDrive.Client.toClientShortInfo(),
                Employee = testDrive.Employee?.toEmployeeShortInfo(),
            };
        }

        public async Task<List<ResponseTestDriveDTO>> GetTestDriveForClient(Guid clientId)
        {
            // 1. Получаем текущее время в UTC и добавляем +3 часа, чтобы получить "время по Москве"
            var nowUtc = DateTime.UtcNow;
            var nowMsk = nowUtc.AddHours(3); // Имитируем московское время

            // 2. Запрашиваем данные из БД — PlannedDate хранится как MSK
            var testDrives = await _context.TestDrives
                .Where(t => t.Status != TestDriveStatus.CANCELED &&
                            t.PlannedDate >= nowMsk && // PlannedDate — это MSK
                            t.PlannedDate <= nowMsk.AddDays(365)
                            && t.ClientId == clientId)
                .Include(td => td.Car)
                .Include(td => td.Client)
                .Include(td => td.Employee)
                .OrderBy(t => t.PlannedDate)
                .ToListAsync();

            if (!testDrives.Any())
                return new List<ResponseTestDriveDTO>();
            // 3. Конвертируем MSK → UTC для DTO (для отправки клиенту)
            var testDrivesListDto = testDrives.Select(t => new ResponseTestDriveDTO
            {
                Id = t.Id,
                Status = t.Status.GetDescription(),
                PlannedDate = t.PlannedDate, // MSK → UTC
                Car = t.Car.toShortInfo(),
                Client = t.Client.toClientShortInfo(),
                Employee = t.Employee?.toEmployeeShortInfo(),
            }).ToList();

            return testDrivesListDto;
        }

        public async Task<List<ResponseTestDriveDTO>> GetTestDriveForManager(Guid managerId)
        {
            // 1. Получаем текущее время в UTC и добавляем +3 часа, чтобы получить "время по Москве"
            var nowUtc = DateTime.UtcNow;
            var nowMsk = nowUtc.AddHours(3); // Имитируем московское время

            // 2. Запрашиваем данные из БД — PlannedDate хранится как MSK
            var testDrives = await _context.TestDrives
                .Where(t => t.Status != TestDriveStatus.CANCELED &&
                            t.PlannedDate >= nowMsk && // PlannedDate — это MSK
                            t.PlannedDate <= nowMsk.AddDays(365)
                            && t.EmployeedId == managerId)
                .Include(td => td.Car)
                .Include(td => td.Client)
                .Include(td => td.Employee)
                .OrderBy(t => t.PlannedDate)
                .ToListAsync();

            if (!testDrives.Any())
                return new List<ResponseTestDriveDTO>();
            // 3. Конвертируем MSK → UTC для DTO (для отправки клиенту)
            var testDrivesListDto = testDrives.Select(t => new ResponseTestDriveDTO
            {
                Id = t.Id,
                Status = t.Status.GetDescription(),
                PlannedDate = t.PlannedDate, // MSK → UTC
                Car = t.Car.toShortInfo(),
                Client = t.Client.toClientShortInfo(),
                Employee = t.Employee?.toEmployeeShortInfo(),
            }).ToList();

            return testDrivesListDto;
        }
    }
}
