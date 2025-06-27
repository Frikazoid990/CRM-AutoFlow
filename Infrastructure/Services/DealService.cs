using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Persistence;
using CRM_AutoFlow.Presentation.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class DealService : IDealService
    {
        private readonly AppDbContext _context;
        private readonly IChatService _chatService;
        private readonly ICarRepository _carRepository;
        public DealService(AppDbContext context, IChatService chatService, ICarRepository carRepository)
        {
            _context = context;
            _chatService = chatService;
            _carRepository = carRepository;
        }

        public async Task<Guid> AddDeal(CreateDealDTO dto)
        {
            // Проверка клиента
            var client = await _context.Users
                .Include(c => c.ClientDeals)
                .FirstOrDefaultAsync(c => c.Id == dto.ClientId);

            if (client == null) throw new ArgumentException("Client not found");

            if (client.ClientDeals.Any(d => !d.IsCancelled && d.Status != DealStatus.COMPLETED))
                throw new ArgumentException("Client already has active deal");

            var car = await _context.Cars
            .FirstOrDefaultAsync(c => c.Id == dto.CarId);

            if (car == null)
                throw new Exception("Car not found");

            // Десериализуем JSON конфигурации
            var configurations = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(car.ConfigurationsJson);
            if (!configurations.ContainsKey(dto.SelectedConfiguration))
                throw new Exception("Configuration not found");

            // Получаем объект выбранной конфигурации
            if (!configurations.TryGetValue(dto.SelectedConfiguration, out var configToken))
            {
                throw new Exception("Configuration not found");
            }

            if (configToken.Type != JTokenType.Object)
            {
                throw new Exception($"Configuration '{dto.SelectedConfiguration}' is not an object. It's a {configToken.Type}");
            }

            var selectedConfig = (JObject)configToken;
            var configDict = selectedConfig.Properties()
    .ToDictionary(p => p.Name.ToLower(), p => p.Value);
            // Извлекаем цену
            decimal price = configDict["price"]?.ToObject<decimal>() ?? 0;

            // Здесь можно добавить логику подсчёта цены по выбранным опциям
            // Например, если цвет или двигатель влияют на цену
            // Но в текущих данных этого нет, поэтому просто проверяем, что опции валидны

            ValidateOptions(configDict, dto.SelectedOptions);

            foreach (var key in dto.SelectedOptions.Keys)
            {
                var values = dto.SelectedOptions[key];

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] is JsonElement element)
                    {
                        // Делаем полную десериализацию элемента в объект или строку
                        values[i] = JsonConvert.DeserializeObject(element.GetRawText());
                    }
                }
            }

            int ChatId = await _chatService.CreateChatAsync(dto.ClientId);

            var deal = new Deal
            {
                Id = Guid.NewGuid(),
                ClientId = dto.ClientId,
                CarId = dto.CarId,
                ChatId = ChatId,
                Price = price,
                Status = DealStatus.NOTASSIGNED,
                SelectedConfiguration = dto.SelectedConfiguration,
                ConfigurationDetailsJson = JsonConvert.SerializeObject(dto.SelectedOptions),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();
            return deal.Id;
        }

        private void ValidateOptions(Dictionary<string, JToken> configDict, Dictionary<string, object[]> options)
        {
            foreach (var option in options)
            {
                var keyLower = option.Key.ToLower();

                if (!configDict.TryGetValue(keyLower, out var token))
                    throw new Exception($"Unknown configuration property: {option.Key}");

                if (token is JArray array)
                {
                    foreach (var value in option.Value)
                    {
                        // Для цвета: десериализуем DTO в CarColor
                        if (option.Key.ToLower() == "color")
                        {
                            var colorDto = JsonConvert.DeserializeObject<CarColor>(value.ToString());

                            bool exists = array
                                .Select(jt => jt.ToObject<CarColor>())
                                .Any(c => c.Name == colorDto.Name && c.Hex == colorDto.Hex);

                            if (!exists)
                                throw new Exception($"Invalid option selected for {option.Key}: {value}");
                        }

                        // Для двигателей: просто строка
                        else if (option.Key.ToLower() == "engine")
                        {
                            var engineStr = value.ToString();
                            if (!array.Any(jt => jt.ToString() == engineStr))
                                throw new Exception($"Invalid option selected for {option.Key}: {engineStr}");
                        }

                        // Другие поля по аналогии
                    }
                }
                else
                {
                    throw new Exception($"Configuration property '{option.Key}' is not an array.");
                }
            }
        }

        public async Task<List<ResponseDealDTO>> GetAllDeals()
        {
            var deals = await _context.Deals
                        .Where(d =>
            d.IsCancelled == false &&
            d.Status == DealStatus.COMPLETED &&
            (DateTime.UtcNow - d.UpdatedAt) < TimeSpan.FromDays(14)
        )
                .Include(d => d.Car)
                .Include(d => d.Client)
                .Include(d => d.Employee)
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();

            if (!deals.Any())
                return new List<ResponseDealDTO>();

            var dealDto = deals.ToResponseDealDtoInList();

            return dealDto;
        }

        public async Task<ResponseDealDTO> GetDealForCliet(Guid clientId)
        {
            var deal = await _context.Deals
                .Where(d => d.ClientId == clientId
                            && !d.IsCancelled
                            && d.Status != DealStatus.COMPLETED)
                .Include(d => d.Car)
                .Include(d => d.Client)
                .Include(d => d.Employee)
                .FirstOrDefaultAsync();
            if (deal == null)
                return null;
            var selectedOptions = JsonConvert.DeserializeObject<Dictionary<string, object[]>>(deal.ConfigurationDetailsJson);

            var dealDto = new ResponseDealDTO
            {
                Id = deal.Id,
                CreatedAt = deal.CreatedAt,
                IsCancelled = deal.IsCancelled,
                Price = deal.Price,
                SelectedOptions = new SelectedOptionsDTO
                {
                    Engine = selectedOptions.GetValueOrDefault("engine")?
                            .Select(e => e.ToString())
                            .ToList(),

                    Color = selectedOptions.GetValueOrDefault("color")?
                            .Select(c => JsonConvert.DeserializeObject<CarColor>(c.ToString()))
                            .ToList()
                },
                SelectedConfiguration = deal.SelectedConfiguration,
                Status = deal.Status.GetDescription(),
                Car = deal.Car.toShortInfo(),
                Client = deal.Client.toClientShortInfo(),
                Employee = deal.Employee.toEmployeeShortInfo(),
            };

            return dealDto;
        }

        public async Task UpdateWithEmploeeDeal(Guid dealId, Guid emploeeId)
        {
            var deal = await _context.Deals.FindAsync(dealId);
            if (deal == null)
            {
                throw new ArgumentException("Deal not found");
            }
            var emploee = await _context.Users
                .Where(u => u.Role != Role.CLIENT)
                .FirstOrDefaultAsync(u => u.Id == emploeeId);
            if (emploee == null)
            {
                throw new ArgumentException("Employee not found");
            }
            deal.EmployeeId = emploeeId;
            deal.UpdatedAt = DateTime.UtcNow;
            deal.Status = DealStatus.NEW;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithStatusDeal(Guid dealId, DealStatus status)
        {
            var deal = await _context.Deals.FindAsync(dealId);
            if (deal == null)
            {
                throw new ArgumentException("Deal not found");
            }
            if (status.Equals(DealStatus.COMPLETED))
                deal.ResolvedAt = DateTime.UtcNow;
            deal.Status = status;
            deal.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithIsCanceled(Guid dealId)
        {
            var deal = await _context.Deals.FindAsync(dealId);
            if (deal == null)
                throw new ArgumentException("Deal not found");
            if (deal.IsCancelled)
                deal.IsCancelled = false;
            else
                deal.IsCancelled = true;
            await _context.SaveChangesAsync();
        }

        public async Task<List<ResponseDealDTO>> GetAllIsCanceledDeal()
        {
            var deals = await _context.Deals
                .Where(d => d.IsCancelled == true)
                .Include(d => d.Car)
                .Include(d => d.Client)
                .Include(d => d.Employee)
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();


            if (!deals.Any())
                return new List<ResponseDealDTO>();
            var dealsDto = deals.ToResponseDealDtoInList();
            return dealsDto;
        }

        public async Task<List<ResponseDealDTO>> GetAllDealsForCurrentManager(Guid managerId)
        {
            var deals = await _context.Deals
                .Where(d =>
                d.EmployeeId == managerId &&
                d.IsCancelled == false &&
                d.Status == DealStatus.COMPLETED &&
                (DateTime.UtcNow - d.UpdatedAt) < TimeSpan.FromDays(14)
                )
                    .Include(d => d.Car)
                    .Include(d => d.Client)
                    .Include(d => d.Employee)
                    .OrderBy(d => d.CreatedAt)
                    .ToListAsync();

            if (!deals.Any())
                return new List<ResponseDealDTO>();

            var dealDto = deals.ToResponseDealDtoInList();

            return dealDto;
        }

    }
}
