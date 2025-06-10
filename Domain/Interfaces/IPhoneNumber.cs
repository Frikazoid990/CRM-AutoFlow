namespace CRM_AutoFlow.Domain.Interfaces
{
    public interface IPhoneNumber
    {
        string CorrectPhoneNumberToDB (string phoneNumber);

        bool isValidRussianPhoneNumber(string phoneNumber);
    }
}
