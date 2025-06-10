using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Models;

namespace CRM_AutoFlow.Presentation.Extensions
{
    public static class TestDriveExtensions
    {
        public static TestDrive ToEntity(this TestDriveDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            return new TestDrive
            {
                ClientId = dto.ClientId,
                CarId = dto.CarId,
                PlannedDate = dto.PlannedDate,
                CreatedAt = dto.CreatedAt,
            };
        }
    }
}
