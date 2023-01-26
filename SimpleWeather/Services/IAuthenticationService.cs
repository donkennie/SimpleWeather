using Microsoft.AspNetCore.Identity;
using SimpleWeather.DTOs;

namespace SimpleWeather.Services
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);
        Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
        Task<TokenDto> CreateToken(bool populateExp);
    }
}
