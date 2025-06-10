using System.ComponentModel.DataAnnotations;

namespace CRM_AutoFlow.Application.DTOs
{
    public class CarDTO
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public Dictionary<string, Dictionary<string, string[]>> Configurations { get; set; }

    }
}
