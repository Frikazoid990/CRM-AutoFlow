using System.ComponentModel.DataAnnotations;

namespace CRM_AutoFlow.Domain.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Guid ClientId { get; set; }
        public Guid? EmployeeId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        //Навигация
        public User Client {  get; set; }
        public User Employee { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public Deal ChatDeal {  get; set; }
    }
}
