using Newtonsoft.Json;
using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;
using System.Text;

namespace SimpleWeather.Repositories.Implementations
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherRepository> _logger;

        public WeatherRepository(ILogger<WeatherRepository> logger, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            this._httpClient = clientFactory.CreateClient();
            _configuration = configuration;
            _logger= logger;
        }

        public async Task<RootObject> GetWeatherForecast(string location)
        {

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://api.openweathermap.org");
            }

            var json = JsonConvert.SerializeObject(location);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            string apiUri = $"data/2.5/weather?q={location}&appid={_configuration["AppSettings:WeatherApiKey"]}";
            var response = await _httpClient.GetAsync(apiUri);
            response.EnsureSuccessStatusCode();

            var stringResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RootObject>(stringResult);
            
        }
    }
}
