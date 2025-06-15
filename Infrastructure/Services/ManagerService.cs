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
    }
}
