using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleWeather.DTOs;
using SimpleWeather.Services;

namespace SimpleWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAuthenticationService _authentication;

        public AuthenticationController(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }

        [HttpPost]
       // [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            var response = await _authentication.CreateAccount(userForRegistration);
            return Ok(response);
        }


        [HttpPost("login")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            var response = await _authentication.ValidateUser(user);
            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetThis()
        {
            return Ok("It works");
        }
    }
}
