using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Models;

namespace CRM_AutoFlow.Presentation.Extensions
{
    public static class DbModelToShortInfoExtensions
    {
        public static CarShortInfoDTO toShortInfo(this Car car)
        {
            return new CarShortInfoDTO
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
            };
        }
        public static EmployeeShortInfoDTO toEmployeeShortInfo(this User user)
        {
            return new EmployeeShortInfoDTO 
            {
                Id = user.Id,
                FullName = user.FullName,
            };
        }
        public static ClientShortInfoDTO toClientShortInfo(this User user)
        {
            return new ClientShortInfoDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
            };
        }
    }
}
