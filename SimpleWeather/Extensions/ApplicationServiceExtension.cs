using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleWeather.Data;
using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;
using SimpleWeather.Repositories.Implementations;
using SimpleWeather.Services;
using System.Text;

namespace SimpleWeather.Extensions
{
    public static class ApplicationServiceExtension
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _config)
        {
            services.AddHttpClient();
            services.AddSingleton<IWeatherRepository, WeatherRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<TokenService>();


            services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.SignIn.RequireConfirmedEmail = true;

            })

          .AddEntityFrameworkStores<DataContext>()
          .AddSignInManager<SignInManager<ApplicationUser>>()
          .AddDefaultTokenProviders();


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(opt =>
               {
                   opt.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = key,
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.Zero
                   };
               });


            services.AddAuthentication();
         

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(_config.GetConnectionString("sqlConnection"));
            });

            return services;
        }


     }
}
