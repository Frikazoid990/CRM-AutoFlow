using System.Text.Json.Serialization;

namespace CRM_AutoFlow.Application.DTOs
{
    public class CreateTestDriveDTO
    {

        public Guid ClientId { get; set; }

        public Guid CarId { get; set; }
        [JsonPropertyName("planedDate")]
        public DateTime PlannedDate { get; set; }


    }
}
