using System.ComponentModel.DataAnnotations;


namespace CRM_AutoFlow.Application.DTOs
{
    using BCrypt.Net;
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; } = Role.CLIENT;
    }
}
