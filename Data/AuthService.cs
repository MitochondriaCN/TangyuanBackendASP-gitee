using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TangyuanBackendASP.Data
{
    public class AuthService
    {
        private readonly IConfiguration _cf;
        public AuthService(IConfiguration configuration)
        {
            _cf = configuration;
        }

        public string GenerateJwtToken(int userId)
        {
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cf["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddDays(3),
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
