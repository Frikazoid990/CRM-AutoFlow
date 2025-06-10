using CRM_AutoFlow.Domain.Interfaces;

namespace CRM_AutoFlow.Infrastructure.Services.PasswordService
{
    public class PasswordService : IPassword
    {

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string hashPassword, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashPassword);
        }
    }
}
