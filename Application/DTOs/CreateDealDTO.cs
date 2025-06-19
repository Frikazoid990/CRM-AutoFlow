using System.ComponentModel.DataAnnotations;

namespace CRM_AutoFlow.Application.DTOs
{
    public class CreateDealDTO
    {
        public Guid ClientId { get; set; }
        public Guid CarId { get; set; }
        public string SelectedConfiguration { get; set; }
        public Dictionary<string, string> SelectedOptions { get; set; }
    }
}
