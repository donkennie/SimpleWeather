using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleWeather.Core;
using SimpleWeather.Features.Requests.Queries;
using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;

namespace SimpleWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleWeatherController : BaseAPIController
    {
        private readonly IWeatherRepository _weatherRepository;

        public SimpleWeatherController( IWeatherRepository weatherRepository)
        {
            _weatherRepository = weatherRepository;
        }

        [HttpGet("location")]
        [ProducesResponseType(typeof(RootObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCompanyById(string location)
        {
            return HandleResult(await Mediator.Send(new GetCurrentWeatherRequest { Location = location }));
        }
    }
}
