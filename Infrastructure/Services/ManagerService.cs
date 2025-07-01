using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class ManagerService : IManagerService
    {
        private readonly AppDbContext _context;

        public ManagerService (AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StaffDto>> GetStaffDealership()
        {
            var staff =  await _context.Users
                    .Where(u => u.Role == Role.MANAGER || u.Role == Role.SENIORMANAGER)
                    .ToListAsync();
            var staffDto = staff.Select(u => new StaffDto
            {
                Id = u.Id,
                FullName = u.FullName,
            }).ToList();
            return staffDto;
        }

        public async Task<StatsManagerDto> GetStatsForManager(Guid managerId)
        {
            var testDrivesInitial = await _context.TestDrives
                .Where(t => t.EmployeedId == managerId && t.Status == TestDriveStatus.INITIAL)
                .ToListAsync();

            var today = DateTime.UtcNow.Date;
            var startOfTheMonth = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var testDrivesCompleted = await _context.TestDrives
                .Where(t => t.EmployeedId == managerId &&
                    t.Status == TestDriveStatus.COMPLETED &&
                    t.CreatedAt >= startOfTheMonth)
                .ToListAsync();

            var dealsCompleted = await _context.Deals
                .Where(d => d.EmployeeId == managerId &&
                d.Status == DealStatus.COMPLETED &&
                d.ResolvedAt >= startOfTheMonth)
                .ToListAsync();
            var dealsNew = await _context.Deals
                .Where(d => d.EmployeeId == managerId &&
                d.Status == DealStatus.NEW)
                .ToListAsync();

            ///Add stats for manager; 
            return new StatsManagerDto
            {
                CompletedDealsCountTotal= dealsCompleted.Count,
                NewDealsCountTotal = dealsNew.Count,
                TestDriveCompletedCountTotal= testDrivesCompleted.Count,
                TestDriveInProcessCountTotal= testDrivesInitial.Count,
                TotalPrice = dealsCompleted.Sum(d=> d.Price),
            };
        }
    }
}
