using Microsoft.IdentityModel.Tokens;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace RS1_2024_25.API.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,$"{user.Name} {user.Surname}"),
                new Claim(ClaimTypes.Role,user.Role.Name),
                new Claim("tenant_id",user.TenantId.ToString())

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds

                );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }



            return Convert.ToBase64String(randomNumber);
        }
        public string GeneratePasswordResetToken()
        {
            return GenerateRefreshToken();
        }
        public string GenerateEmailConfirmationToken()
        {
            return GenerateRefreshToken();
        }

        


    }
}
