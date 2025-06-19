using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Persistence;
using CRM_AutoFlow.Presentation.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class DealService : IDealService
    {
        private readonly AppDbContext _context;
        private readonly IChatService _chatService;
        public DealService(AppDbContext context, IChatService chatService)
        {
            _context = context;
            _chatService = chatService;
        }

        public async Task<Guid> AddDeal(CreateDealDTO dealDto)
        {
            var client = await _context.Users
                .Include(c => c.ClientDeals)
                .FirstOrDefaultAsync(c => c.Id == dealDto.ClientId);
            if (client == null)
            {
                throw new ArgumentException("Client not found");
            }
            var hasActiveDeals = client.ClientDeals.Any(d =>
                d.Status != DealStatus.COMPLETED && !d.IsCancelled);
            if (hasActiveDeals)
            {
                throw new ArgumentException("The client already has an active deal");
            }

            var car = await _context.Cars.FindAsync(dealDto.CarId);
            if (car == null)
            {
                throw new ArgumentException("Car not found");
            }

            var configurations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(car.ConfigurationsJson);

            if (!configurations.ContainsKey(dealDto.SelectedConfiguration))
                throw new ArgumentException("Invalid configuration selected");

            var configDetails = configurations[dealDto.SelectedConfiguration];


            foreach (var option in dealDto.SelectedOptions)
            {
                // Проверяем наличие ключа
                if (!configDetails.ContainsKey(option.Key))
                    throw new ArgumentException($"Option {option.Key} not available");

                // Проверяем наличие значения в списке
                if (!configDetails[option.Key].Contains(option.Value))
                    throw new ArgumentException($"Invalid {option.Value} for {option.Key}");
            }

            int ChatId = await _chatService.CreateChatAsync(dealDto.ClientId);
            var deal = new Deal
            {
                ClientId = dealDto.ClientId,
                CarId = dealDto.CarId,
                ChatId = ChatId,
                Status = DealStatus.NOTASSIGNED,
                Price = decimal.Parse(configDetails["Price"].First()),
                SelectedConfiguration = dealDto.SelectedConfiguration,
                ConfigurationDetailsJson = JsonSerializer.Serialize( new
                {
                    dealDto.SelectedOptions
                })
            };
            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();
            return (deal.Id);
        }

        public async Task<List<ResponseDealDTO>> GetAllDeals()
        {
            var deals = await _context.Deals
                .Where(d => d.IsCancelled == false)
                .Include(d => d.Car)
                .Include (d => d.Client)
                .Include(d => d.Employee)
                .ToListAsync();
             
            if (!deals.Any())
                return new List<ResponseDealDTO>();

            var dealDto = deals.Select(d => new ResponseDealDTO
            {
                Id = d.Id,
                CreatedAt = d.CreatedAt,
                IsCancelled = d.IsCancelled,
                Price = d.Price,
                Status = d.Status.GetDescription(),
                SelectedConfiguration = d.SelectedConfiguration,
                SelectedOptions = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(d.ConfigurationDetailsJson)?
                    .GetValueOrDefault("SelectedOptions")
                ?? new Dictionary<string, string>(),
                Car = d.Car.toShortInfo(),
                Client = d.Client.toClientShortInfo(),
                Employee = d.Employee.toEmployeeShortInfo(),
            }).ToList();

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
            deal.Status = status;
            deal.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithIsCanceled(Guid dealId)
        {
            var deal = await _context.Deals.FindAsync(dealId);
            if (deal == null)
                throw new ArgumentException("Deal not found");
            if(deal.IsCancelled)
                deal.IsCancelled = false;
            else
                deal.IsCancelled = true;
            await _context.SaveChangesAsync();
        }

    }
}
