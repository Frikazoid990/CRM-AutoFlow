using System.ComponentModel.DataAnnotations;

namespace CRM_AutoFlow.Domain.Models
{
    public class TestDrive
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ClientId { get; set; }
        [Required]
        public Guid CarId { get; set; }
        public Guid? EmployeedId { get; set; }
        [Required]
        public DateTime PlannedDate {  get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public TestDriveStatus Status { get; set; } = TestDriveStatus.NOTASSIGNED;

        //Навигационные св-ва
        public User Client {  get; set; }
        public User Employee { get; set; }
        public Car Car {  get; set; }

    }
}
