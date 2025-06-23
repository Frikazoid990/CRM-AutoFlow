using System.ComponentModel.DataAnnotations;

namespace CRM_AutoFlow.Application.DTOs
{
    public class CarDTO
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public Dictionary<string, CarConfiguration> Configurations { get; set; }
        public string ImgPath { get; set; }
        public string Description { get; set; }

    }

    public class CarConfiguration
    {
        public List<string> Engine {  get; set; }

        public decimal Price { get; set; }

        public List<CarColor> Color { get; set; }
    }

    public class CarColor
    {
        public string Name { get; set; }
        public string Hex { get; set; }
    }
}
