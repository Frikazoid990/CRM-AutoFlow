using Bogus;
using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Domain.Models;
using CRM_AutoFlow.Infrastructure.Persistence;
using CRM_AutoFlow.Presentation.Extensions;
using Form_Registration_App.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace CRM_AutoFlow.Infrastructure.Services
{
    public class ClassterDb
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        private readonly IChatService _chatService;

        public ClassterDb(AppDbContext context, UserService userService, IChatService chatService)
        {
            _context = context;
            _userService = userService;
            _chatService = chatService;
        }
        public async Task CreateData()
        {
            var faker = new Faker("ru");
            var userDtoList = new List<UserDTO>();
            var random = new Random();
            // Расширенные списки отчеств
            var malePatronymics = new[] {
        "Иванович", "Петрович", "Сергеевич", "Алексеевич", "Дмитриевич",
        "Андреевич", "Михайлович", "Владимирович", "Евгеньевич", "Анатольевич",
        "Николаевич", "Олегович", "Борисович", "Викторович", "Геннадьевич",
        "Юрьевич", "Станиславович", "Вадимович", "Георгиевич", "Романович",
        "Артемович", "Васильевич", "Данилович", "Константинович", "Леонидович"
    };

            var femalePatronymics = new[] {
        "Ивановна", "Петровна", "Сергеевна", "Алексеевна", "Дмитриевна",
        "Андреевна", "Михайловна", "Владимировна", "Евгеньевна", "Анатольевна",
        "Николаевна", "Олеговна", "Борисовна", "Викторовна", "Геннадьевна",
        "Юрьевна", "Станиславовна", "Вадимовна", "Георгиевна", "Романовна",
        "Артемовна", "Васильевна", "Даниловна", "Константиновна", "Леонидовна"
    };

            for (int i = 0; i < 2; i++)
            {
                // Получаем "Имя Фамилия" от Bogus
                var bogusName = faker.Name.FullName();
                var nameParts = bogusName.Split(' ');


                // Если Bogus вернул только одно слово, добавляем фамилию
                if (nameParts.Length == 1)
                {
                    nameParts = new[] { nameParts[0], faker.Name.LastName() };
                }

                // Определяем пол по окончанию имени
                var firstName = nameParts[0];
                bool isMale = !(firstName.EndsWith("а") || firstName.EndsWith("я"));

                // Выбираем случайное отчество
                var patronymic = isMale
                    ? faker.PickRandom(malePatronymics)
                    : faker.PickRandom(femalePatronymics);

                // Формируем полное ФИО в русском формате "Фамилия Имя Отчество"
                var fullNameWithPatronymic = $"{nameParts[1]} {nameParts[0]} {patronymic}";

                // Генерация номера телефона
                var phoneNumber = "+7" + random.Next(900, 999) + // код оператора (900-999)
                         random.Next(1000000, 9999999).ToString(); // остальные цифры

                var userDto = new UserDTO
                {
                    FullName = fullNameWithPatronymic,
                    Password = "test",
                    PhoneNumber = phoneNumber,
                };

                userDtoList.Add(userDto);

                // Для отладки выводим в консоль

            }
            var cars = await _context.Cars.ToListAsync();
            var employee = await _context.Users
                .Where(u => u.Role == Role.SENIORMANAGER || u.Role == Role.MANAGER)
                .ToListAsync();

            foreach (var userDto in userDtoList)
            {
                //Console.WriteLine($"fn: {userDto.FullName} | pn:{userDto.PhoneNumber} | pass:{userDto.Password}");
                var randomCar = cars[random.Next(cars.Count)];
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var configurations = JsonSerializer.Deserialize<Dictionary<string, CarConfiguration>>(randomCar.ConfigurationsJson, options);
                if (configurations != null && configurations.Any())
                {
                    var randomConfigKey = configurations.Keys.ElementAt(random.Next(configurations.Count));

                    var randomConfig = configurations[randomConfigKey];

                    var randomColor = randomConfig.Color[random.Next(randomConfig.Color.Count)];

                    var randomEngine = randomConfig.Engine[random.Next(randomConfig.Engine.Count)];

                    var selectedConfig = new
                    {
                        engine = new List<string> { randomEngine },
                        color = new List<object> { new { name = randomColor.Name, hex = randomColor.Hex } }
                    };

                    //int ChatId = await _chatService.CreateChatAsync(ClientId); // in prodaction
                    var statusValues = Enum.GetValues(typeof(DealStatus));
                    var randomStatus = (DealStatus)statusValues.GetValue(random.Next(statusValues.Length));

                    var createdAt = DateTime.UtcNow.AddDays(-random.Next(365));
                    var updatedAt = createdAt.AddDays(random.Next(0, (DateTime.UtcNow - createdAt).Days));
                    DateTime? resolvedAt = null;

                    if (randomStatus == DealStatus.COMPLETED)
                    {
                        resolvedAt = updatedAt.AddDays(random.Next(1, 30));
                    }

                    var deal = new Deal
                    {
                        Id = Guid.NewGuid(),
                        ClientId = Guid.NewGuid(), // Здесь должен быть реальный ID клиента
                        ChatId = random.Next(1000, 9999), // Пример случайного ChatId
                        CarId = randomCar.Id,
                        Price = randomConfig.Price,
                        Status = randomStatus,
                        IsCancelled = randomStatus != DealStatus.COMPLETED && random.Next(2) == 0, // 10% chance to be cancelled
                        CreatedAt = createdAt,
                        UpdatedAt = updatedAt,
                        ResolvedAt = resolvedAt,
                        SelectedConfiguration = randomConfigKey,
                        ConfigurationDetailsJson = JsonSerializer.Serialize(selectedConfig)
                    };

                    // Назначаем сотрудника только если статус не NOTASSIGNED
                    if (randomStatus != DealStatus.NOTASSIGNED && employee.Any())
                    {
                        deal.EmployeeId = employee[random.Next(employee.Count)].Id;
                    }
                    Console.WriteLine(deal);
                    Console.WriteLine($"""
    ============ Информация о сделке ============
    ID: {deal.Id}
    Клиент: {deal.ClientId}
    Чат: {deal.ChatId}
    Автомобиль: {deal.CarId}
    Сотрудник: {(deal.EmployeeId.HasValue ? deal.EmployeeId.Value.ToString() : "не назначен")}
    Цена: {deal.Price} руб.
    Статус: {deal.Status.GetDescription()}
    Отменена: {(deal.IsCancelled ? "Да" : "Нет")}
    Дата создания: {deal.CreatedAt:dd.MM.yyyy HH:mm}
    Дата обновления: {deal.UpdatedAt:dd.MM.yyyy HH:mm}
    Дата завершения: {(deal.ResolvedAt.HasValue ? deal.ResolvedAt.Value.ToString("dd.MM.yyyy HH:mm") : "не завершена")}
    Комплектация: {deal.SelectedConfiguration}
    Детали: {deal.ConfigurationDetailsJson}
    =============================================
    """);
                }
                else
                {
                    Console.WriteLine("У выбранного автомобиля нет конфигураций");
                }

            }






        }
    }
}
