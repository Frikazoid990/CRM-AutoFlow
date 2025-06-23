using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Application.Helper;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Persistence;
using CRM_AutoFlow.Presentation.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Text.Json;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class CarService : ICarRepository
    {
        private readonly AppDbContext _context;
        public CarService(AppDbContext context) 
        {
            _context = context;
        }
        public async Task<IEnumerable> GetAllCars()
        {
            var cars = await _context.Cars.ToListAsync();

            var result = cars.Select(u => {
                var car = new CarDTO()
                {
                    Id = u.Id,
                    Brand = u.Brand,
                    Model = u.Model,
                    Configurations = JsonSerializer.Deserialize< Dictionary<string, CarConfiguration>>(u.ConfigurationsJson),
                    ImgPath = u.ImgPath,
                    Description = u.Description
                };
                return car;
                });
            

            return result;
        }

        public async Task<Result<Guid>>  AddCar(CarDTO carDto)
        {
            var car = carDto.ToEntity();
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
            return Result<Guid>.Ok(car.Id);
        }

    }
}
