using CRM_AutoFlow.API.Configurations;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Infrastructure.Hubs;
using CRM_AutoFlow.Infrastructure.Persistence;
using CRM_AutoFlow.Infrastructure.Services;
using CRM_AutoFlow.Infrastructure.Services.PasswordService;
using Form_Registration_App.Services;
using FormRegJWTAndDB.Auth;
using FormRegJWTAndDB.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddCustomAuthConfiguration(builder.Configuration)
    .AddCustomSwagger()
    .AddCorsPolitics()
    .AddControllers();// Добавление контроллеров

builder.Services.AddSignalR(); // add signalR for chat

//Добавление сторонних сервисов
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IDealService, DealService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ITestDrive, TestDriveService>();
//builder.Services.AddScoped<ClassterDb>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<IPhoneNumber, PhoneNumberService>(); 
builder.Services.AddScoped<ICarRepository, CarService>();
builder.Services.AddScoped<IPassword, PasswordService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
//
builder.Services.AddAuthorization(); //Добавление в сервис авторизацию

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)); // Для PostgreSQL

//Конфигурация приложения
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var classterDb = services.GetRequiredService<ClassterDb>();
        await classterDb.CreateData(); // Вызываем ваш метод
        Console.WriteLine("Данные успешно созданы!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при создании данных: {ex.Message}");
    }
}

app.UseCors("AllowFrontend"); //Включение CORS политики 

app.UseAuthentication(); //Использование аутонтефикации
app.UseAuthorization(); //Использование авторизации

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapHub<ChatHub>("chat");
//Запуск приложения

app.Run();

