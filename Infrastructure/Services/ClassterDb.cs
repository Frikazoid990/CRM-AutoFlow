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
            var dealsList = new List<Deal>();
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

            for (int i = 0; i < 1; i++)
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

            //create Deals
            var shuffled = userDtoList.OrderBy(u => Guid.NewGuid()).ToList();
            var selectedUsersDto = shuffled.Take(2).ToList();
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
                        IsCancelled = randomStatus != DealStatus.COMPLETED && random.Next(20) == 0, // 10% chance to be cancelled
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


            Console.WriteLine("==============================================================" +
                "==============================================================" +
                "==============================================================" +
                "==============================================================" +
                "==============================================================");

            //create TestDrives
            // Фиксируем текущую дату (23.06.2025)
            var currentDate = new DateTime(2025, 6, 23);

            // Словарь для хранения занятых слотов
            var bookedSlots = new Dictionary<Guid, List<DateTime>>();

            foreach (var userDto in userDtoList)
            {
                // Определяем случайное количество тест-драйвов для пользователя (1-4)
                int testDriveCount = random.Next(1, 5);

                Console.WriteLine($"Создаем {testDriveCount} тест-драйв(ов) для {userDto.FullName}");

                for (int i = 0; i < testDriveCount; i++)
                {
                    var randomCar = cars[random.Next(cars.Count)];

                    if (!bookedSlots.ContainsKey(randomCar.Id))
                    {
                        bookedSlots[randomCar.Id] = new List<DateTime>();
                    }

                    // Генерируем временные слоты (от 2 месяцев назад до 2 месяцев вперед)
                    var startTime = new TimeSpan(9, 0, 0);
                    var endTime = new TimeSpan(17, 30, 0);
                    var timeSlots = new List<DateTime>();

                    // Создаем слоты в диапазоне ±2 месяца от текущей даты
                    for (int day = -60; day <= 60; day++) // -60 дней = ~2 месяца назад, +60 дней = ~2 месяца вперед
                    {
                        var date = currentDate.AddDays(day);

                        // Пропускаем выходные (суббота и воскресенье)
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                            continue;

                        // Для сегодняшней даты проверяем текущее время
                        if (day == 0)
                        {
                            var now = currentDate.TimeOfDay;
                            var start = now > startTime ? now.Add(new TimeSpan(1, 0, 0)) : startTime;
                            startTime = new TimeSpan(start.Hours, start.Minutes / 30 * 30, 0);
                        }

                        for (var time = startTime; time <= endTime; time = time.Add(new TimeSpan(0, 30, 0)))
                        {
                            timeSlots.Add(date.Add(time));
                        }
                    }

                    // Фильтруем доступные слоты
                    var availableSlots = timeSlots
                        .Where(slot => !bookedSlots[randomCar.Id].Any(booked =>
                            Math.Abs((booked - slot).TotalMinutes) < 30))
                        .ToList();

                    if (!availableSlots.Any())
                    {
                        Console.WriteLine($"  Нет свободных слотов для {randomCar.Brand} {randomCar.Model}");
                        continue;
                    }

                    var plannedDate = availableSlots[random.Next(availableSlots.Count)];
                    bookedSlots[randomCar.Id].Add(plannedDate);

                    // Определяем статус в зависимости от даты
                    TestDriveStatus randomStatus;
                    Guid? employeeId = null;

                    if (plannedDate < currentDate)
                    {
                        // Прошедшие даты - только COMPLETED или CANCELED
                        var statuses = new[] { TestDriveStatus.COMPLETED, TestDriveStatus.CANCELED };
                        randomStatus = statuses[random.Next(statuses.Length)];

                        // Для завершенных назначаем сотрудника
                        if (randomStatus == TestDriveStatus.COMPLETED && employee.Any())
                        {
                            employeeId = employee[random.Next(employee.Count)].Id;
                        }
                    }
                    else if (plannedDate.Date == currentDate.Date)
                    {
                        // Сегодня - может быть INITIAL или CANCELED
                        var statuses = new[] { TestDriveStatus.INITIAL, TestDriveStatus.CANCELED };
                        randomStatus = statuses[random.Next(statuses.Length)];

                        if (randomStatus == TestDriveStatus.INITIAL && employee.Any())
                        {
                            employeeId = employee[random.Next(employee.Count)].Id;
                        }
                    }
                    else
                    {
                        // Будущие даты - NOTASSIGNED или INITIAL
                        var statuses = new[] { TestDriveStatus.NOTASSIGNED, TestDriveStatus.INITIAL };
                        randomStatus = statuses[random.Next(statuses.Length)];

                        if (randomStatus == TestDriveStatus.INITIAL && employee.Any())
                        {
                            employeeId = employee[random.Next(employee.Count)].Id;
                        }
                    }

                    // Создаем тест-драйв
                    var testDrive = new TestDrive
                    {
                        Id = Guid.NewGuid(),
                        ClientId = Guid.NewGuid(),
                        CarId = randomCar.Id,
                        EmployeedId = employeeId,
                        PlannedDate = plannedDate,
                        CreatedAt = currentDate.AddDays(-random.Next(60)), // Создан до 2 месяцев назад
                        UpdatedAt = currentDate,
                        Status = randomStatus
                    };

                    // Корректируем UpdatedAt для завершенных/отмененных
                    if (randomStatus == TestDriveStatus.COMPLETED || randomStatus == TestDriveStatus.CANCELED)
                    {
                        testDrive.UpdatedAt = plannedDate.AddMinutes(random.Next(30, 180));
                    }

                    _context.TestDrives.Add(testDrive);

                    // Вывод информации
                    Console.WriteLine($"  Тест-драйв #{i + 1}:");
                    Console.WriteLine($"  Авто: {randomCar.Brand} {randomCar.Model}");
                    Console.WriteLine($"  Дата: {plannedDate:dd.MM.yyyy HH:mm}");
                    Console.WriteLine($"  Статус: {randomStatus}");
                    Console.WriteLine($"  Сотрудник: {employeeId?.ToString() ?? "не назначен"}");
                    Console.WriteLine("  ------------------------------");
                }
            }

            Console.WriteLine("====================================================================================" +
                "====================================================================================" +
                "====================================================================================" +
                "====================================================================================" +
                "====================================================================================");
        }
    }
}
