namespace CRM_AutoFlow.Application.DTOs
{
    public class ResponseDealDTO
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsCancelled { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; }

        public string SelectedConfiguration { get; set; }

        public Dictionary<string,string> SelectedOptions { get; set; }

        public CarShortInfoDTO Car { get; set; }

        public ClientShortInfoDTO Client { get; set; }

        public EmployeeShortInfoDTO? Employee { get; set; }

    }
}
