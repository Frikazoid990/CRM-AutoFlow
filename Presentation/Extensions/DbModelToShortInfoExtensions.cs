using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Models;

namespace CRM_AutoFlow.Presentation.Extensions
{
    public static class DbModelToShortInfoExtensions
    {
        public static CarShortInfoDTO toShortInfo(this Car car)
        {
            if (car == null) 
                return null;
            return new CarShortInfoDTO
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                ImgPath = car.ImgPath,
            };
        }
        public static EmployeeShortInfoDTO toEmployeeShortInfo(this User user)
        {
            if (user == null)
                return null;
            return new EmployeeShortInfoDTO 
            {
                Id = user.Id,
                FullName = user.FullName,
            };
        }
        public static ClientShortInfoDTO toClientShortInfo(this User user)
        {
            if (user == null)
                return null;
            return new ClientShortInfoDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
            };
        }
    }
}
