using CRM_AutoFlow.Infrastructure.Persistence.ConfigurationsModelsToDb;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CRM_AutoFlow.Domain.Models
{
    public class Car
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string ConfigurationsJson { get; set; }
        [Required]
        public string ImgPath { get; set; }
        [Required]
        public string Description { get; set; }

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
        public ICollection<TestDrive> TestDrives { get; set; } = new List<TestDrive>();

    }
}
