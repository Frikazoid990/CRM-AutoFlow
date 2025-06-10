using CRM_AutoFlow.Domain.Models;

namespace FormRegJWTAndDB.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
