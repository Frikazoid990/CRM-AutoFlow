namespace CRM_AutoFlow.Application.DTOs
{
    public class CreateMessageDto
    {
        public int ChatId { get; set; }

        public Guid SenderId { get; set; }

        public string Content { get; set; }
    }
}
