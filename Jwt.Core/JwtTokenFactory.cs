using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Jwt.Core
{
    public interface IJwtTokenFactory
    {
        string Create(string userId);
    }

    public class JwtTokenFactory : IJwtTokenFactory
    {
        private readonly string _key;

        public JwtTokenFactory(string key)
        {
            _key = key;
        }

        public string Create(string userId)
        {
            var configSecurityKey = Encoding.UTF8.GetBytes(_key);
            var securityKey = new SymmetricSecurityKey(configSecurityKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, userId),
                }),
                SigningCredentials = credentials,
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}