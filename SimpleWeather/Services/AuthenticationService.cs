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
                        Message = "User exist",
                        IsSuccessful = false,
                       // StatusCode = StatusCodes.Status400BadRequest;
                    };
                }

                if (await _userManager.Users.AnyAsync(x => x.UserName == registerDTO.Username.ToLower()))
                {
                    return new BaseCommandResponse()
                    {
                        Message = "UsernameExist",
                        IsSuccessful = false,
                       // StatusCode = StatusCodes.RecordExist
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
                       // StatusCode = StatusCodes.GeneralError,
                        Message = string.Join("\n", result.Errors.Select(e => e.Description).ToArray()),
                        IsSuccessful = false
                    };

                }

                return new BaseCommandResponse()
                {
                    // StatusCode = StatusCodes.Successful,
                    Message = "Successful ",
                    IsSuccessful = true
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering ::: ", ex.Message);

                return new BaseCommandResponse()
                {
                   // StatusCode = StatusCodes.GeneralError,
                    Message = "RegistrationError",
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
                        Message = "NoUserExists",
                        IsSuccessful = false,
                       // StatusCode = StatusCodes.NoRecordFound
                    };
                }

                var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

                if (!checkPassword.Succeeded)
                {
                    return new LoginResponseDTO
                    {
                        Message = "NoUserExists",
                        IsSuccessful = false,
                        //StatusCode = StatusCodes.GeneralError
                    };
                }

                return new LoginResponseDTO
                {
                    Token = _tokenService.CreateToken(user),
                    Username = user.UserName,
                    Message = "Successful",
                    IsSuccessful = true,
                 //   StatusCode = StatusCodes.Successful
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Login Exception ::: ", ex.Message);

                return new LoginResponseDTO
                {
                   // StatusCode = StatusCodes.GeneralError,
                    Message = "ExceptionMessage",
                    IsSuccessful = false
                };
            }
        }





        /* public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
         {
             var validateUser = _userManager.Users.AnyAsync(x => x.UserName == userForRegistration.Username);

             ApplicationUser applicationUser = new ApplicationUser
             {
                 Email = userForRegistration.Email,
                 UserName = userForRegistration.Username.ToLower(),
             };

             var result = await _userManager.CreateAsync(applicationUser,
             userForRegistration.Password);
             return result;
         }


         public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
         {
             _user = await _userManager.FindByNameAsync(userForAuth.UserName);
             var result = (_user != null && await _userManager.CheckPasswordAsync(_user,
            userForAuth.Password));
             if (!result)
                 _logger.LogWarning($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");
             return result;
         }

         public async Task<TokenDto> CreateToken(bool populateExp)
         {
             var signingCredentials = GetSigningCredentials();
             var claims = await GetClaims();
             var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
             await _userManager.UpdateAsync(_user);
             var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
             return new TokenDto(accessToken);

         }

         private SigningCredentials GetSigningCredentials()
         {
             var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("TokenKey"));
             var secret = new SymmetricSecurityKey(key);
             return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
         }

         private async Task<List<Claim>> GetClaims()
         {
             var claims = new List<Claim>
          {
          new Claim(ClaimTypes.Name, _user.UserName)
          };
             var roles = await _userManager.GetRolesAsync(_user);
             foreach (var role in roles)
             {
                 claims.Add(new Claim(ClaimTypes.Role, role));
             }
             return claims;
         }


         private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
         {
             var tokenOptions = new JwtSecurityToken
             (
            issuer: _jwtConfiguration.ValidIssuer,
              audience: _jwtConfiguration.ValidAudience,
              claims: claims,
              expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
             signingCredentials: signingCredentials
             );

             return tokenOptions;
         }

         private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
         {
             var tokenValidationParameters = new TokenValidationParameters
             {
                 ValidateAudience = true,
                 ValidateIssuer = true,
                 ValidateIssuerSigningKey = true,
                 IssuerSigningKey = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"))),
                 ValidateLifetime = true,
                 ValidIssuer = _jwtConfiguration.ValidIssuer,
                 ValidAudience = _jwtConfiguration.ValidAudience
             };
             var tokenHandler = new JwtSecurityTokenHandler();
             SecurityToken securityToken;
             var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out
            securityToken);
             var jwtSecurityToken = securityToken as JwtSecurityToken;
             if (jwtSecurityToken == null ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
             StringComparison.InvariantCultureIgnoreCase))
             {
                 throw new SecurityTokenException("Invalid token");
             }
             return principal;
         }
        */

    }

}