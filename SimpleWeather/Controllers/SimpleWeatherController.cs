using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleWeather.Core;
using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;

namespace SimpleWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleWeatherController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherRepository _weatherRepository;

        public SimpleWeatherController(ILogger<WeatherForecastController> logger, IWeatherRepository weatherRepository)
        {
            _logger = logger;
            _weatherRepository = weatherRepository;
        }

        [HttpGet]
        public async Task <ActionResult<RootObject>> GetCurrentWeather(string location)
        {
            if (location == null)
                return BadRequest("Something is wrong somewhere");
            return await _weatherRepository.GetWeatherForecast(location);
        }
    }
}
