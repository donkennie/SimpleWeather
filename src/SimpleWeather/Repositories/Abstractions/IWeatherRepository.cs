using SimpleWeather.Models;

namespace SimpleWeather.Repositories.Abstractions
{
    public interface IWeatherRepository
    {
        Task<RootObject> GetWeatherForecast(string location);
    }
}
