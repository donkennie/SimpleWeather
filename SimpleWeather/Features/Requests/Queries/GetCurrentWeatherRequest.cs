using MediatR;
using SimpleWeather.Core;
using SimpleWeather.Models;

namespace SimpleWeather.Features.Requests.Queries
{
    public class GetCurrentWeatherRequest : IRequest<ResultResponse<RootObject>>
    {
        public string? Location { get; set; }
    }
}
