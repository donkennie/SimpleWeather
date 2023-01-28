using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleWeather.Core;
using SimpleWeather.Features.Requests.Queries;
using SimpleWeather.Models;
using SimpleWeather.Repositories.Abstractions;

namespace SimpleWeather.Controllers
{

    public class SimpleWeatherController : BaseAPIController
    {

        [HttpGet("location")]
        [ProducesResponseType(typeof(RootObject), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrentWeatherByLocation(string location)
        {
            return HandleResult(await Mediator.Send(new GetCurrentWeatherRequest { Location = location }));
        }
    }
}
