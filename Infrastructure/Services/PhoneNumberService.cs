using CRM_AutoFlow.Domain.Interfaces;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class PhoneNumberService : IPhoneNumber
    {
        public string CorrectPhoneNumberToDB(string phoneNumber)
        {
            if (phoneNumber.StartsWith("8"))
            {
                phoneNumber = "7" + phoneNumber.Substring(1);
            }
            if (!phoneNumber.StartsWith("+"))
            {
                phoneNumber = "+" + phoneNumber;
            }

            return phoneNumber;
        }
        public bool isValidRussianPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Убираем все символы, кроме цифр
            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Проверяем длину и начало номера
            if (digits.Length != 11) return false;
            // Должен начинаться с 7 или 8
            return digits.StartsWith("7");

        }
    }
}
