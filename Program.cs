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
    .AddControllers();// ���������� ������������

builder.Services.AddSignalR(); // add signalR for chat

//���������� ��������� ��������
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
builder.Services.AddAuthorization(); //���������� � ������ �����������

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)); // ��� PostgreSQL

//������������ ����������
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var classterDb = services.GetRequiredService<ClassterDb>();
        await classterDb.CreateData(); // �������� ��� �����
        Console.WriteLine("������ ������� �������!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"������ ��� �������� ������: {ex.Message}");
    }
}

app.UseCors("AllowFrontend"); //��������� CORS �������� 

app.UseAuthentication(); //������������� ��������������
app.UseAuthorization(); //������������� �����������

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapHub<ChatHub>("chat");
//������ ����������

app.Run();

