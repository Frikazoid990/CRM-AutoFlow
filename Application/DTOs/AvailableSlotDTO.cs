namespace CRM_AutoFlow.Application.DTOs
{
    public class AvailableSlotDTO
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCarAvailable { get; set; }
        public bool IsStaffAvailable { get; set; }

        public string Label => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
    }
}
