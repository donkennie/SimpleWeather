using Newtonsoft.Json;
using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;
using System.Text;
using System.Text.Json;

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
           // string airQuality = isAirQualityNeeded ? "yes" : "no";
            string apiUri = $"?key={_configuration["AppSettings:WeatherApiKey"]}&q={location}";
            string key = "f36517855d6bd3d9b3bab51c8369783c";
            //HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"/data/2.5/weather?q={location}&appid={key}&units=metric");
            //RootObject rawWeather = new RootObject();
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://api.openweathermap.org");
            }
            var json = JsonConvert.SerializeObject(location);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await this._httpClient.GetAsync($"?key={_configuration["AppSettings:WeatherApiKey"]}&q={location}");
            response.EnsureSuccessStatusCode();

            var stringResult = await response.Content.ReadAsStringAsync();
             return JsonConvert.DeserializeObject<RootObject>(stringResult);
            //return System.Text.Json.JsonSerializer.Deserialize<RootObject>(stringResult);
            /* if (httpResponseMessage.IsSuccessStatusCode)
             {
                 _logger.LogTrace(2003, "Request is successful");
             }
             else
             {
                 _logger.LogWarning(2004, "Request is un-successful");
                 return null;
             }*/
        }
    }
}
