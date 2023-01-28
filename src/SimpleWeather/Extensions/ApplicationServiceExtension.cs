using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleWeather.Data;
using SimpleWeather.Features.Handlers.Queries;
using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;
using SimpleWeather.Repositories.Implementations;
using SimpleWeather.Services;
using System.Text;
using MediatR;
using Microsoft.OpenApi.Models;

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
            services.AddScoped<IUserAccessor, UserAccessor>();


            services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.SignIn.RequireConfirmedEmail = false;

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

            services.AddMediatR(typeof(GetCurrentWeatherRequestHandler).Assembly);


            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Current Weather API",
                    Version = "v1",
                    Description = "Current Weather API by Donkennie",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Ajeigbe Kehinde",
                        Email = "ajeigbekehinde160@gmail.com",
                        Url = new Uri("https://twitter.com/donkennie"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Current Weather API LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
             {
             {
             new OpenApiSecurityScheme
             {
             Reference = new OpenApiReference
             {
             Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
            },
            Name = "Bearer",

            },

             new List<string>()

                }

             });


            });

            return services;
        }


     }
}
