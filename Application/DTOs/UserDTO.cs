using System.ComponentModel.DataAnnotations;


namespace CRM_AutoFlow.Application.DTOs
{
    public class UserDTO
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
