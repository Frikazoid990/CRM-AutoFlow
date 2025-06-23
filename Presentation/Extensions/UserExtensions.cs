using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Services.PasswordService;
using Microsoft.AspNetCore.Identity;

namespace CRM_AutoFlow.Presentation.Extensions
{
    public static class UserExtensions
    {

        public static User ToEntity(this UserDTO dto, IPassword passwordHasher)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (passwordHasher == null)
                throw new ArgumentNullException(nameof(passwordHasher));

            return new User()
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHasher.HashPassword(dto.Password),
                Role = Role.CLIENT,
            };
        }

    }
}
