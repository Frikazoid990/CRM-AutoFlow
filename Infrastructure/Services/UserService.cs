
using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Application.Helper;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Persistence;
using CRM_AutoFlow.Presentation.Extensions;
using FormRegJWTAndDB.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Form_Registration_App.Services
{
    public class UserService
    {
        private readonly IPassword _passwordService;
        private readonly AppDbContext _context;
        private readonly IPhoneNumber _phoneNumber;
        private readonly ITokenService _tokenService;

        public UserService(AppDbContext context, IPassword passwordService, IPhoneNumber phoneNumber, ITokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _phoneNumber = phoneNumber;
            _tokenService = tokenService;
        }

        public async Task<Result<string>> SignInUser(string phoneNumber, string password)
        {
            phoneNumber = _phoneNumber.CorrectPhoneNumberToDB(phoneNumber);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user is null)
                return Result<string>.Fail("User is not found.");
            if (!_passwordService.Verify(user.PasswordHash, password))
                return Result<string>.Fail("Invalid password.");
            var token = _tokenService.GenerateToken(user);
            return Result<string>.Ok(token);

        }

        public async Task<Result<Guid>> AddUser(UserDTO userDto)
        {
            var phoneNumber = _phoneNumber.CorrectPhoneNumberToDB(userDto.PhoneNumber);
            userDto.PhoneNumber = phoneNumber;
            if (!_phoneNumber.isValidRussianPhoneNumber(userDto.PhoneNumber))
                return Result<Guid>.Fail("Invalid phone number.");
            if (userDto is null)
                return Result<Guid>.Fail("User is null.");
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == userDto.PhoneNumber))
                return Result<Guid>.Fail("The user with this phone number already exists.");
            var user = userDto.ToEntity(_passwordService);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Result<Guid>.Ok(user.Id);
        }
    }
}
