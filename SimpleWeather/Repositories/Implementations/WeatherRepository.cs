using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;
using System.Text.Json;

namespace SimpleWeather.Repositories.Implementations
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherRepository> _logger;

        public WeatherRepository(ILogger<WeatherRepository> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient= httpClient;
            _configuration= configuration;
            _logger= logger;
        }

        public async Task<RootObject> GetWeatherForecast(string location)
        {
            string apiUri = $"?key={_configuration["AppSettings:WeatherApiKey"]}&q={location}";

            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(apiUri);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogTrace(2003, "Request is successful");
                return JsonSerializer.Deserialize<RootObject>(await httpResponseMessage.Content.ReadAsStringAsync());
            }
            else
            {
                _logger.LogWarning(2004, "Request is un-successful");
                return null;
            }
        }
    }
}
