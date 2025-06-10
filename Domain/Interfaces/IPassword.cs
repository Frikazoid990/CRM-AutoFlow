namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface IPassword
    {
        string HashPassword(string password);
        bool Verify(string hashPassword, string providedPassword);
    }
}
