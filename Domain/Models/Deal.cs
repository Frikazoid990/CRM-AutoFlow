using System.ComponentModel.DataAnnotations;

namespace CRM_AutoFlow.Domain.Models
{
    public class Deal
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ClientId { get; set; }
        [Required]
        public int ChatId {  get; set; }
        public Guid? EmployeeId { get; set; }
        [Required]
        public Guid CarId { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DealStatus Status { get; set; } = DealStatus.INITIAL;
        [Required]
        public bool IsCancelled { get; set; } = false;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

        // Навигационные свойства
        public User Client { get; set; }
        public User Employee { get; set; }
        public Car Car { get; set; }
        public Chat Chat { get; set; }
    }
}
