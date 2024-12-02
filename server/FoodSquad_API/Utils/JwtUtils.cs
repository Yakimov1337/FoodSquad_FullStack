using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodSquad_API.Models.Enums;
using Microsoft.IdentityModel.Tokens;

namespace FoodSquad_API
{
    // JwtUtil for token generation and validation
    public class JwtUtil
    {
        private readonly string _secret;
        public readonly long AccessTokenExpiration;
        public readonly long RefreshTokenExpiration;

        public JwtUtil(IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("Jwt");
            _secret = jwtConfig["Key"] ?? throw new InvalidOperationException("JWT Secret is missing.");
            AccessTokenExpiration = long.Parse(jwtConfig["AccessTokenExpirationMinutes"]) * 60 * 1000;
            RefreshTokenExpiration = long.Parse(jwtConfig["RefreshTokenExpirationDays"]) * 24 * 60 * 60 * 1000;
        }

        public string GenerateToken(string email, UserRole role, Guid id, string name, string phoneNumber, string imageUrl, long expiration)
        {
            var claims = new List<Claim>
    {
        new Claim("email", email),
        new Claim(ClaimTypes.Role, role.ToString()),
        new Claim("id", id.ToString()),
        new Claim("name", name),
        new Claim("phoneNumber", phoneNumber ?? ""),
        new Claim("imageUrl", imageUrl ?? "")
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMilliseconds(expiration),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }






        public IDictionary<string, object> ExtractClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                throw new SecurityTokenException("Invalid token");

            return jwtToken.Claims.ToDictionary(c => c.Type, c => (object)c.Value);
        }

        public TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
