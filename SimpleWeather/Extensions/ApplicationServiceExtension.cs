using Polly;
using SimpleWeather.Repositories.Abstractions;
using SimpleWeather.Repositories.Implementations;

namespace SimpleWeather.Extensions
{
    public static class ApplicationServiceExtension
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _config)
        {
            //AddHttpClient::Enabling Single connection for the service using IHttpClientFactory. It can also be used for handling multiple APIs
            services.AddHttpClient<IWeatherRepository, WeatherRepository>(w =>
            {
                w.BaseAddress = new Uri(_config["AppSettings:WeatherApiBaseAddress"]);
            })
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(10, _ => TimeSpan.FromSeconds(5))) //To handle 5XX and 408 http errors
                .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(5))) //To handle continuous Bad Requests
                .AddPolicyHandler(request =>
                {
                    if (request.Method == HttpMethod.Get)
                    {
                        return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(Convert.ToInt32(_config["AppSettings:GetMethodTimeOut"])));
                    }
                    return Policy.NoOpAsync<HttpResponseMessage>();
                }); //To limit the timeout of the request based on its type.
            return services;
        }


     }
}
