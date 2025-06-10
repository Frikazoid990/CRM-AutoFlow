namespace CRM_AutoFlow.Application.DTOs
{
    public class AvailableDayDTO
    {
        public DateTime Date { get; set; }
        public List<AvailableSlotDTO> Slots { get; set; } = new();
        public bool HasAvailableSlots => Slots.Any();
    }
}
