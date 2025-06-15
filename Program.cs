using CRM_AutoFlow.API.Configurations;
using CRM_AutoFlow.Domain.Interfaces;
using CRM_AutoFlow.Infrastructure.Persistence;
using CRM_AutoFlow.Infrastructure.Services;
using CRM_AutoFlow.Infrastructure.Services.PasswordService;
using Form_Registration_App.Services;
using FormRegJWTAndDB.Auth;
using FormRegJWTAndDB.Interfaces;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services
    .AddCustomAuthConfiguration(builder.Configuration)
    .AddCustomSwagger()
    .AddCorsPolitics()
    .AddControllers(); // Добавление контроллеров

//Добавление сторонних сервисов
builder.Services.AddScoped<ITestDrive, TestDriveService>();
builder.Services.AddScoped<IPhoneNumber,PhoneNumberService>();
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
app.UseCors("AllowFrontend"); //Включение CORS политики 

app.UseAuthentication(); //Использование аутонтефикации
app.UseAuthorization(); //Использование авторизации

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

//Запуск приложения
app.Run();
