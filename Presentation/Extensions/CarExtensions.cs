using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Models;
using System.Text.Json;

namespace CRM_AutoFlow.Presentation.Extensions
{
    public static class CarExtensions
    {
        public static Car ToEntity(this CarDTO carDto)
        {
            if (carDto == null)
                throw new ArgumentNullException(nameof(carDto));

            return new Car()
            {
                Brand = carDto.Brand,
                Model = carDto.Model,
                ConfigurationsJson = JsonSerializer.Serialize(carDto.Configurations),
                Description = carDto.Description,
                ImgPath = carDto.ImgPath,
            };
            
        }

    }
}
