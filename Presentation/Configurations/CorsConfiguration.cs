namespace CRM_AutoFlow.API.Configurations
{
    public static class CorsConfiguration
    {
        public static IServiceCollection AddCorsPolitics(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") // Адрес фронтенда
                         .AllowAnyMethod()                     // Разрешить все HTTP-методы (GET, POST и т.д.)
                         .AllowAnyHeader()                   // Разрешить все заголовки
                         .AllowCredentials(); // Разрешаем credentials
                });
            });
            
            return services;
        }
    }
}
