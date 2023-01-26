using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleWeather.Core;
using SimpleWeather.DTOs;
using SimpleWeather.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SimpleWeather.Services
{
    internal sealed class AuthenticationService : IAuthenticationService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ILogger<AuthenticationService> _logger;
        private readonly TokenService _tokenService;

        public AuthenticationService( ILogger<AuthenticationService> logger, TokenService tokenService, SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
           
        }

        public async Task<BaseCommandResponse> CreateAccount(UserForRegistrationDto registerDTO)
        {
            try
            {
                if (await _userManager.Users.AnyAsync(x => x.Email == registerDTO.Email))
                {
                    return new BaseCommandResponse()
                    {
                        Message = "Email exist",
                        IsSuccessful = false,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                if (await _userManager.Users.AnyAsync(x => x.UserName == registerDTO.Username.ToLower()))
                {
                    return new BaseCommandResponse()
                    {
                        Message = "Username Exist",
                        IsSuccessful = false,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                ApplicationUser applicationUser = new ApplicationUser
                {
                    Email = registerDTO.Email,
                    UserName = registerDTO.Username.ToLower(),
                };

                var result = await _userManager.CreateAsync(applicationUser, registerDTO.Password);

                if (!result.Succeeded)
                {
                    return new BaseCommandResponse()
                    {
                        StatusCode = StatusCodes.Status503ServiceUnavailable,
                        Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()),
                        IsSuccessful = false
                    };
                }

                return new BaseCommandResponse()
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Account created successful!",
                    IsSuccessful = true
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering ::: ", ex.Message);

                return new BaseCommandResponse()
                {
                    StatusCode = StatusCodes.Status503ServiceUnavailable,
                    Message = "Registration Error",
                    IsSuccessful = false
                };
            }
        }



        public async Task<LoginResponseDTO> ValidateUser(UserForAuthenticationDto loginDTO)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDTO.UserName);

                if (user is null)
                {
                    return new LoginResponseDTO
                    {
                        Message = "No User Exists",
                        IsSuccessful = false,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }

                var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

                if (!checkPassword.Succeeded)
                {
                    return new LoginResponseDTO
                    {
                        Message = "Wrong authentication",
                        IsSuccessful = false,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                return new LoginResponseDTO
                {
                    Token = _tokenService.CreateToken(user),
                    Username = user.UserName,
                    Message = "Login Successful!",
                    IsSuccessful = true,
                    StatusCode = StatusCodes.Status200OK
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Login Exception ::: ", ex.Message);

                return new LoginResponseDTO
                {
                    StatusCode = StatusCodes.Status503ServiceUnavailable,
                    Message = "Exception Message",
                    IsSuccessful = false
                };
            }
        }
      
    }
}