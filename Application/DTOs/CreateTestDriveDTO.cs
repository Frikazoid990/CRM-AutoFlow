namespace CRM_AutoFlow.Application.DTOs
{
    public class CreateTestDriveDTO
    {
        public Guid Id { get; set; }

        public Guid ClientId { get; set; }

        public Guid CarId { get; set; }

        public DateTime PlannedDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
