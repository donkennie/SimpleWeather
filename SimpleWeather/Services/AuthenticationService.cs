using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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

        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly JwtConfiguration _jwtConfiguration;
        private ILogger<AuthenticationService> _logger;
        private User? _user;

        public AuthenticationService( IMapper mapper, ILogger<AuthenticationService> logger,
        UserManager<User> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _jwtConfiguration = new JwtConfiguration();
            _configuration.Bind(_jwtConfiguration.Section, _jwtConfiguration);

        }

        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);
            var result = await _userManager.CreateAsync(user,
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
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));
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


    }

}