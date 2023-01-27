using Microsoft.AspNetCore.Identity;
using SimpleWeather.Core;
using SimpleWeather.DTOs;

namespace SimpleWeather.Services
{
    public interface IAuthenticationService
    {
        Task<BaseCommandResponse> CreateAccount(UserForRegistrationDto registerDTO);
        Task<LoginResponseDTO> ValidateUser(UserForAuthenticationDto loginDTO);

    }
}
