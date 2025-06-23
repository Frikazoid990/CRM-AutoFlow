namespace CRM_AutoFlow.Application.DTOs
{
    public class CreateTestDriveDTO
    {

        public Guid ClientId { get; set; }

        public Guid CarId { get; set; }

        public DateTime PlannedDate { get; set; }


    }
}
