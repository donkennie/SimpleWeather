using Newtonsoft.Json;
using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

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
            string key = "c3f51b7ae7c04e48a1f41541232501";
            string apiUri = $"?key=c3f51b7ae7c04e48a1f41541232501&q={location}&aqi=no";
            // string apiUri = $"http://api.openweathermap.org/data/2.5/weather?q={location}&appid={key}";
            //string apiUri = "https://api.openweathermap.org/data/2.5/weather?lat=44.34&lon=10.99&appid={key}";
            //HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"/data/2.5/weather?q={location}&appid={key}&units=metric");
            //RootObject rawWeather = new RootObject();
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://api.openweathermap.org");
            }
           // var json = JsonConvert.SerializeObject(location);
           // var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.GetAsync(apiUri);
            response.EnsureSuccessStatusCode();

            var stringResult = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RootObject>(stringResult);
            
        }
    }
}
