using System.ComponentModel.DataAnnotations;

namespace CRM_AutoFlow.Domain.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int ChatId { get; set; }
        [Required]
        public Guid SenderId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        //Навигация
        public Chat Chat { get; set; }
        public User Sender { get; set; }
    }
}
