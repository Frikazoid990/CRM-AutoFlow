using CRM_AutoFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CRM_AutoFlow.Domain.Models
{

    public class User
    {

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public Role Role { get; set; }

        //Сделки
        public ICollection<Deal> ClientDeals { get; set; } = new List<Deal>();
        public ICollection<Deal> EmployeeDeals { get; set; } = new List<Deal>();
        //Тест-драйвы
        public ICollection<TestDrive> ClientTestDrives { get; set; } = new List<TestDrive>();
        public ICollection<TestDrive> EmployeeTestDrives { get; set; } = new List<TestDrive>();
        //Чат
        public ICollection<Chat> ClientChats { get; set; } = new List<Chat>();
        public ICollection<Chat> EmployeeChats { get; set; } = new List<Chat>();
        //Сообщения
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
