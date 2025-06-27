using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Models;
using Newtonsoft.Json;

namespace CRM_AutoFlow.Presentation.Extensions
{
    public static class DealExtensions
    {
        public static List<ResponseDealDTO> ToResponseDealDtoInList(this List<Deal> deals)
        {
            var dealDto = deals.Select(deal =>
            {
                var selectedOptions = JsonConvert.DeserializeObject<Dictionary<string, object[]>>(deal.ConfigurationDetailsJson);

                return new ResponseDealDTO
                {
                    Id = deal.Id,
                    CreatedAt = deal.CreatedAt,
                    IsCancelled = deal.IsCancelled,
                    Price = deal.Price,
                    Status = deal.Status.GetDescription(),
                    SelectedConfiguration = deal.SelectedConfiguration,

                    SelectedOptions = new SelectedOptionsDTO
                    {
                        Engine = selectedOptions.GetValueOrDefault("engine")?
                            .Select(e => e.ToString())
                            .ToList(),

                        Color = selectedOptions.GetValueOrDefault("color")?
                            .Select(c => JsonConvert.DeserializeObject<CarColor>(c.ToString()))
                            .ToList()
                    },

                    Car = deal.Car.toShortInfo(),
                    Client = deal.Client.toClientShortInfo(),
                    Employee = deal.Employee?.toEmployeeShortInfo(),
                };
            }).ToList();
            return dealDto;
        }

        public static ResponseDealDTO ToResponseDealDto(this Deal deal)
        {
            var selectedOptions = JsonConvert.DeserializeObject<Dictionary<string, object[]>>(deal.ConfigurationDetailsJson);

            return new ResponseDealDTO
            {
                Id = deal.Id,
                CreatedAt = deal.CreatedAt,
                IsCancelled = deal.IsCancelled,
                Price = deal.Price,
                Status = deal.Status.GetDescription(),
                SelectedConfiguration = deal.SelectedConfiguration,

                SelectedOptions = new SelectedOptionsDTO
                {
                    Engine = selectedOptions.GetValueOrDefault("engine")?
                            .Select(e => e.ToString())
                            .ToList(),

                    Color = selectedOptions.GetValueOrDefault("color")?
                            .Select(c => JsonConvert.DeserializeObject<CarColor>(c.ToString()))
                            .ToList()
                },

                Car = deal.Car.toShortInfo(),
                Client = deal.Client.toClientShortInfo(),
                Employee = deal.Employee?.toEmployeeShortInfo(),
            };
        }
    }
}
