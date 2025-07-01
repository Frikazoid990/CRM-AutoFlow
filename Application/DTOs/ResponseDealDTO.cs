using CRM_AutoFlow.Application.DTOs;

namespace CRM_AutoFlow.Application.DTOs
{
    public class ResponseDealDTO
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int ChatId { get; set; }

        public bool IsCancelled { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; }

        public string SelectedConfiguration { get; set; }

        public SelectedOptionsDTO SelectedOptions { get; set; }

        public CarShortInfoDTO Car { get; set; }

        public ClientShortInfoDTO Client { get; set; }

        public EmployeeShortInfoDTO? Employee { get; set; }


    }
}

public class SelectedOptionsDTO
{
    public List<string> Engine { get; set; }
    public List<CarColor> Color { get; set; }
}
