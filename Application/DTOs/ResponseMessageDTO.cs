namespace CRM_AutoFlow.Application.DTOs
{
    public class ResponseMessageDTO
    {
        public Guid Id { get; set; }

        public string Sendler { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
