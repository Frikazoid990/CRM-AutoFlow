using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CRM_AutoFlow.API.Configurations
{
    public static class AuthConfiguration
    {
        public static IServiceCollection AddCustomAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtTokenConfiguration:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtTokenConfiguration:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtTokenConfiguration:SecretKey"])),
                    ValidateIssuerSigningKey = true
                };
            });
            return services;
        }
    }
}
