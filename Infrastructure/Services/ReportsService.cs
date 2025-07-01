using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class ReportsService : IReportsService
    {
        private readonly AppDbContext _context;

        public ReportsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CarSalesReport>> ReportCar(DateTime DateStart, DateTime DateEnd)
        {
            var completedDeals = await _context.Deals
                .Where(d => d.Status == DealStatus.COMPLETED &&
                            !d.IsCancelled &&
                            d.ResolvedAt >= DateStart &&
                            d.ResolvedAt <= DateEnd)
                .Include(d => d.Car) // Чтобы получить данные о марке и модели
                .ToListAsync();

            var groupedResults = completedDeals
                .GroupBy(d => new { d.Car.Id, d.Car.Brand, d.Car.Model })
                .Select(g => new CarSalesReport
                {
                    id = g.Key.Id.ToString(),
                    modelBrand = $"{g.Key.Brand} {g.Key.Model}",
                    unitsSold = g.Count(),
                    totalRevenue = g.Sum(d => d.Price),
                    avgPrice = g.Average(d => d.Price)
                })
                .ToList();

            var report = groupedResults
                .Select((item, index) => new CarSalesReport
                {
                    id = (index + 1).ToString(), // если нужен string
                                                 // или (int id): id = index + 1; если тип id будет int
                    modelBrand = item.modelBrand,
                    unitsSold = item.unitsSold,
                    totalRevenue = item.totalRevenue,
                    avgPrice = item.avgPrice,
                })
                .ToList();

            return report;
        }

        public async Task<List<ManagerPerformanceReport>> ReportManager(DateTime DateStart, DateTime DateEnd)
        {
            // Шаг 1: Получаем всех менеджеров и старших менеджеров
            var managers = await _context.Users
                .Where(u => u.Role == Role.MANAGER || u.Role == Role.SENIORMANAGER)
                .Include(u => u.EmployeeDeals) // Сделки, где он сотрудник
                .Include(u => u.EmployeeTestDrives) // Тест-драйвы, где он сотрудник
                .ToListAsync();

            var reportItems = new List<ManagerPerformanceReport>();

            foreach (var manager in managers)
            {
                // Завершённые тест-драйвы за период
                var completedTestDrives = manager.EmployeeTestDrives
                    .Where(td => td.Status == TestDriveStatus.COMPLETED &&
                                 td.UpdatedAt >= DateStart &&
                                 td.UpdatedAt <= DateEnd)
                    .ToList();

                // Созданные сделки за период
                var registeredDeals = manager.EmployeeDeals
                    .Where(d => d.CreatedAt >= DateStart &&
                                d.CreatedAt <= DateEnd)
                    .ToList();

                // Завершённые сделки за период
                var completedDeals = manager.EmployeeDeals
                    .Where(d => d.Status == DealStatus.COMPLETED &&
                                !d.IsCancelled &&
                                d.ResolvedAt >= DateStart &&
                                d.ResolvedAt <= DateEnd)
                    .ToList();

                decimal revenue = completedDeals.Sum(d => d.Price);

                string conversionRate = "0%";
                if (registeredDeals.Count > 0)
                {
                    double rate = (double)completedDeals.Count / registeredDeals.Count * 100;
                    conversionRate = $"{rate:F2}%";
                }

                reportItems.Add(new ManagerPerformanceReport
                {
                    managerFullName = manager.FullName,
                    testDrives = completedTestDrives.Count,
                    dealsRegistered = registeredDeals.Count,
                    dealsCompleted = completedDeals.Count,
                    conversionRate = conversionRate,
                    revenue = revenue.ToString("F2")
                });
            }

            // Теперь присваиваем id как последовательный номер
            var report = reportItems
                .Select((item, index) => new ManagerPerformanceReport
                {
                    id = (index + 1).ToString(), // Вот так будет 1, 2, 3...
                    managerFullName = item.managerFullName,
                    testDrives = item.testDrives,
                    dealsRegistered = item.dealsRegistered,
                    dealsCompleted = item.dealsCompleted,
                    conversionRate = item.conversionRate,
                    revenue = item.revenue
                })
                .ToList();

            return report;
        }

        public async Task<List<FunnelReport>> ReportFunnel(DateTime DateStart, DateTime DateEnd)
        {
            // Шаг 1: Все тест-драйвы в периоде
            var allTestDrives = await _context.TestDrives
                .Where(td => td.CreatedAt >= DateStart && td.CreatedAt <= DateEnd)
                .ToListAsync();

            int totalTestDrives = allTestDrives.Count;

            // Шаг 2: Завершённые тест-драйвы
            var completedTestDrives = allTestDrives
                .Where(td => td.Status == TestDriveStatus.COMPLETED)
                .ToList();

            int completedCount = completedTestDrives.Count;

            // Шаг 3: Клиенты, которые делали Completed тест-драйв и потом купили эту же машину
            var clientCarPairs = completedTestDrives
                .Select(td => new { td.ClientId, td.CarId })
                .Distinct()
                .ToList();

            var clientsWithDealsOnSameCar = new List<Deal>();

            foreach (var pair in clientCarPairs)
            {
                var deals = await _context.Deals
                    .Where(d =>
                        d.ClientId == pair.ClientId &&
                        d.CarId == pair.CarId &&
                        d.CreatedAt >= DateStart && d.CreatedAt <= DateEnd &&
                        d.Status == DealStatus.COMPLETED &&
                        !d.IsCancelled)
                    .ToListAsync();

                if (deals.Any())
                {
                    clientsWithDealsOnSameCar.AddRange(deals);
                }
            }

            int clientsWithDealsCount = clientCarPairs
                .Where(pair => clientsWithDealsOnSameCar
                    .Any(d => d.ClientId == pair.ClientId && d.CarId == pair.CarId))
                .Count();

            // Шаг 4: Успешные сделки
            int successfulDealsCount = clientsWithDealsOnSameCar.Count;

            // Формируем финальный отчёт
            var funnel = new List<FunnelReport>
    {
        new FunnelReport
        {
            id = "1",
            stage = "Все тест-драйвы",
            count = totalTestDrives,
            percentage = 100,
            color = "#FF9999"
        },
        new FunnelReport
        {
            id = "2",
            stage = "Успешные тест-драйвы",
            count = completedCount,
            percentage = totalTestDrives > 0 ? (int)Math.Round((double)completedCount / totalTestDrives * 100) : 0,
            color = "#66B2FF"
        },
        new FunnelReport
        {
            id = "3",
            stage = "Зарегистрированные сделки",
            count = clientsWithDealsCount,
            percentage = completedCount > 0 ? (int)Math.Round((double)clientsWithDealsCount / completedCount * 100) : 0,
            color = "#99FF99"
        },
        new FunnelReport
        {
            id = "4",
            stage = "Завершенные сделки",
            count = successfulDealsCount,
            percentage = clientsWithDealsCount > 0 ? (int)Math.Round((double)successfulDealsCount / clientsWithDealsCount * 100) : 0,
            color = "#FFD700"
        }
    };

            return funnel;
        }
    }
}
